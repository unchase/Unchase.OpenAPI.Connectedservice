using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xaml;
using Unchase.OpenAPI.ConnectedService.Common;

namespace Unchase.OpenAPI.ConnectedService.Converters
{
    [TypeConverter(typeof(SharedImageSourceExtensionConverter))]
    [MarkupExtensionReturnType(typeof(Uri))]
    public class SharedImageSourceExtension : MarkupExtension, IUriContext
    {
#if VS17
        private const string AssemblyName = "Unchase.OpenAPI.Connectedservice.VS22";
#else
        private const string AssemblyName = "Unchase.OpenAPI.ConnectedService";
#endif
        private const string SharedRootNamespace = "Unchase.OpenAPI.Connectedservice.Shared";
        private static readonly Uri PackAppBaseUri = PackUriHelper.Create(new Uri("application://"));
        private const string _packageApplicationBaseUriEscaped = "application:///";
        private const string COMPONENT = ";component";
        private const string WrongFirstSegment = "The required pattern for URI containing \";component\" is \"AssemblyName;Vxxxx;PublicKey;component\", where Vxxxx is the assembly version and PublicKey is the 16-character string representing the assembly public key token. Vxxxx and PublicKey are optional.";

        #region Designer reflection utilities
        private const string WpfViewNodeManagerDesignTimePropertiesTypeName = "Microsoft.VisualStudio.DesignTools.WpfDesigner.Metadata.WpfViewNodeManagerDesignTimeProperties";
        private const string WpfMiscDesignTimePropertiesTypeName = "Microsoft.VisualStudio.DesignTools.WpfDesigner.Metadata.WpfMiscDesignTimeProperties";
        private const string WpfDesignerAssemblyName = "Microsoft.VisualStudio.DesignTools.WpfDesigner";
        private const string IViewNodeTypeName = "Microsoft.VisualStudio.DesignTools.Platform.InstanceBuilders.IViewNode";
        private const string IDocumentContextTypeName = "Microsoft.VisualStudio.DesignTools.Markup.DocumentModel.IDocumentContext";
        private const string IDocumentRootResolverTypeName = "Microsoft.VisualStudio.DesignTools.Markup.DocumentModel.IDocumentRootResolver";
        private const string IInstanceBuilderContextTypeName = "Microsoft.VisualStudio.DesignTools.Designer.InstanceBuilders.IInstanceBuilderContext";
        private const string ContextPropertyName = "Context";
        private const string DocumentContextPropertyName = "DocumentContext";
        private const string DocumentNodePropertyName = "DocumentNode";
        private const string DocumentPathPropertyName = "DocumentPath";
        private static DependencyProperty s_viewNodeProperty;
        private static DependencyProperty s_instanceBuilderContextProperty;
        private static PropertyInfo s_piDocumentNode;
        private static PropertyInfo s_piDocumentPath;
        private static Type s_typeDocumentNode;
        private static Type s_typeIInstanceBuilderContext;
        private static PropertyInfo s_piContext;
        private static PropertyInfo s_piDocumentContext;
        private static MethodInfo s_miMakeDesignTimeUri;

        private static DependencyProperty ViewNodeProperty
        {
            get
            {
                if (s_viewNodeProperty == null)
                {
                    Type designType = Type.GetType(WpfViewNodeManagerDesignTimePropertiesTypeName + ", " + WpfDesignerAssemblyName);
                    PropertyInfo piViewNodeProperty = designType?.GetProperty("ViewNodeProperty", BindingFlags.Public | BindingFlags.Static);
                    s_viewNodeProperty = (DependencyProperty)piViewNodeProperty?.GetValue(null);
                }
                return s_viewNodeProperty;
            }
        }

        private static DependencyProperty InstanceBuilderContextProperty
        {
            get
            {
                if (s_instanceBuilderContextProperty == null)
                {
                    Type designType = Type.GetType(WpfMiscDesignTimePropertiesTypeName + ", " + WpfDesignerAssemblyName);
                    PropertyInfo piDP = designType?.GetProperty("InstanceBuilderContextProperty", BindingFlags.Public | BindingFlags.Static);
                    s_instanceBuilderContextProperty = (DependencyProperty)piDP?.GetValue(null);
                }
                return s_instanceBuilderContextProperty;
            }
        }

        private static object GetDocumentNodeFromViewNode(object viewNode)
        {
            if (s_piDocumentNode == null)
            {
                Type typeIViewNode = viewNode.GetType().GetInterface(IViewNodeTypeName);
                if (typeIViewNode == null)
                    throw new InvalidCastException($"{viewNode.GetType()} does not implement the interface {IViewNodeTypeName}.");
                s_piDocumentNode = typeIViewNode.GetProperty(DocumentNodePropertyName);
                if (s_piDocumentNode == null)
                    throw new MissingMemberException(IViewNodeTypeName, DocumentNodePropertyName);
            }
            return s_piDocumentNode.GetValue(viewNode);
        }

        private static object GetContextFromDocumentNode(object docNode)
        {
            Type nodeType = docNode.GetType();
            if (s_piContext == null || nodeType != s_typeDocumentNode)
            {
                s_typeDocumentNode = nodeType;
                s_piContext = nodeType.GetProperty(ContextPropertyName);
                if (s_piContext == null)
                    throw new MissingMemberException(nodeType.FullName, ContextPropertyName);
            }
            return s_piContext.GetValue(docNode);
        }

        private static object GetDocumentContextFromInstanceBuilderContext(object ibc)
        {
            if (s_piDocumentContext == null)
            {
                Type typeIInstanceBuilderContext = ibc.GetType().GetInterface(IInstanceBuilderContextTypeName);
                if (typeIInstanceBuilderContext == null)
                    throw new InvalidCastException($"{ibc.GetType()} does not implement the interface {IInstanceBuilderContextTypeName}.");
                s_piDocumentContext = typeIInstanceBuilderContext.GetProperty(DocumentContextPropertyName);
                if (s_piDocumentContext == null)
                    throw new MissingMemberException(IInstanceBuilderContextTypeName, DocumentContextPropertyName);
            }
            return s_piDocumentContext.GetValue(ibc);
        }

        private static string GetDocumentPathFromDesignerObject(object viewNodeOrInstanceBuilderContext)
        {
            object context;
            if (IsInstanceBuilderContext(viewNodeOrInstanceBuilderContext))
                context = GetDocumentContextFromInstanceBuilderContext(viewNodeOrInstanceBuilderContext);
            else
            {
                object docNode = GetDocumentNodeFromViewNode(viewNodeOrInstanceBuilderContext);
                context = GetContextFromDocumentNode(docNode);
            }
            if (s_piDocumentPath == null)
            {
                Type typeIDocumentContext = context.GetType().GetInterface(IDocumentContextTypeName);
                if (typeIDocumentContext == null)
                    throw new InvalidCastException($"{context.GetType()} does not implement the interface {IDocumentContextTypeName}.");
                s_piDocumentPath = typeIDocumentContext.GetProperty(DocumentPathPropertyName);
                if (s_piDocumentPath == null)
                    throw new MissingMemberException(IDocumentContextTypeName, DocumentPathPropertyName);
            }
            return (string)s_piDocumentPath.GetValue(context);
        }

        private static bool IsInstanceBuilderContext(object obj)
        {
            Type objType = obj.GetType();
            if (s_typeIInstanceBuilderContext == null)
                s_typeIInstanceBuilderContext = objType.GetInterface(IInstanceBuilderContextTypeName);
            return s_typeIInstanceBuilderContext == null ? false : s_typeIInstanceBuilderContext.IsAssignableFrom(objType);
        }

        private static Uri MakeDesignTimeUri(object viewNodeOrInstanceBuilderContext, Uri uri)
        {
            object context;
            if (IsInstanceBuilderContext(viewNodeOrInstanceBuilderContext))
                context = GetDocumentContextFromInstanceBuilderContext(viewNodeOrInstanceBuilderContext);
            else
            {
                object docNode = GetDocumentNodeFromViewNode(viewNodeOrInstanceBuilderContext);
                context = GetContextFromDocumentNode(docNode);
            }
            if (s_miMakeDesignTimeUri == null)
            {
                Type typeIDocumentRootResolver = context.GetType().GetInterface(IDocumentRootResolverTypeName);
                if (typeIDocumentRootResolver == null)
                    throw new InvalidCastException($"{context.GetType()} does not implement the interface {IDocumentRootResolverTypeName}.");
                s_miMakeDesignTimeUri = typeIDocumentRootResolver.GetMethod(nameof(MakeDesignTimeUri), new Type[] { typeof(Uri) });
                if (s_miMakeDesignTimeUri == null)
                    throw new MissingMemberException(IDocumentRootResolverTypeName, nameof(MakeDesignTimeUri));
            }
            return (Uri)s_miMakeDesignTimeUri.Invoke(context, new object[] { uri });
        }

        private interface IUnwrapHelper
        {
            object Unwrap(object reference);
        }

        private class WeakReferenceHelper<T> : IUnwrapHelper where T : class
        {
            private T Unwrap(object reference)
            {
                WeakReference weakReference = reference as WeakReference;
                if (weakReference != null && weakReference.IsAlive)
                {
                    return (T)weakReference.Target;
                }
                return reference as T;
            }

            object IUnwrapHelper.Unwrap(object reference)
            {
                return Unwrap(reference);
            }
        }

        private static IUnwrapHelper s_unwrapViewNode;
        private static IUnwrapHelper UnwrapViewNodeHelper
        {
            get
            {
                if (s_unwrapViewNode == null && ViewNodeProperty != null)
                {
                    Type typeUnwrapHelper = typeof(WeakReferenceHelper<>).MakeGenericType(ViewNodeProperty.PropertyType);
                    s_unwrapViewNode = (IUnwrapHelper)Activator.CreateInstance(typeUnwrapHelper);
                }
                return s_unwrapViewNode;
            }
        }
        private static IUnwrapHelper s_UnwrapInstanceBuilderContext;
        private static IUnwrapHelper UnwrapInstanceBuilderContextHelper
        {
            get
            {
                if (s_UnwrapInstanceBuilderContext == null && InstanceBuilderContextProperty != null)
                {
                    Type typeUnwrapHelper = typeof(WeakReferenceHelper<>).MakeGenericType(InstanceBuilderContextProperty.PropertyType);
                    s_UnwrapInstanceBuilderContext = (IUnwrapHelper)Activator.CreateInstance(typeUnwrapHelper);
                }
                return s_UnwrapInstanceBuilderContext;
            }
        }
        private static object UnwrapViewNode(object reference)
        {
            return UnwrapViewNodeHelper?.Unwrap(reference);
        }
        private static object UnwrapInstanceBuilderContext(object reference)
        {
            return UnwrapInstanceBuilderContextHelper?.Unwrap(reference);
        }

        #endregion

        public Uri BaseUri { get; set; }

        [ConstructorArgument("uriSource")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Uri UriSource { get; set; }

        public SharedImageSourceExtension()
        {
        }

        public SharedImageSourceExtension(Uri uriSource)
        {
            UriSource = uriSource ?? throw new ArgumentNullException(nameof(uriSource));
        }

        private static T RequireService<T>(IServiceProvider provider) where T : class
        {
            T t = provider.GetService(typeof(T)) as T;
            if (t == null)
                throw new InvalidOperationException($"The {nameof(SharedImageSourceExtension)} needs a service of type {typeof(T).Name}.");
            return t;
        }

        private static T GetService<T>(IServiceProvider provider) where T : class
        {
            return (T)provider.GetService(typeof(T));
        }

        private static string UriToString(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            return uri.GetComponents(uri.IsAbsoluteUri ? UriComponents.AbsoluteUri : UriComponents.SerializationInfoString, UriFormat.SafeUnescaped);
        }

        private static ResourceDictionary GetResourceDictionary(object obj)
        {
            if (obj is FrameworkElement)
                return ((FrameworkElement)obj).Resources;
            if (obj is FrameworkContentElement)
                return ((FrameworkContentElement)obj).Resources;
            if (obj is FrameworkTemplate)
                return ((FrameworkTemplate)obj).Resources;
            if (obj is Style)
                return ((Style)obj).Resources;
            return null;
        }

        private static object GetViewNodeFromSP(IServiceProvider serviceProvider)
        {
            var irop = GetService<IRootObjectProvider>(serviceProvider);
            if (irop != null)
            {
                DependencyProperty viewNodeProperty = ViewNodeProperty;
                ResourceDictionary resources = GetResourceDictionary(irop.RootObject);
                object viewNode = resources != null ? resources[viewNodeProperty] : null;
                if (viewNode == null)
                {
                    DependencyObject rootDO = irop.RootObject as DependencyObject;
                    viewNode = rootDO.GetValue(viewNodeProperty);
                }
                viewNode = UnwrapViewNode(viewNode);
                if (viewNode == null)
                    throw new InvalidOperationException("The Designer object can't be found. This is required to convert design-time URI in relative URI.");

                return viewNode;
            }
            return null;
        }

        private static void EnsureProvideValueTarget(IServiceProvider serviceProvider, out BitmapImage imgTarget, out DependencyProperty propUriSource)
        {
            var ipvt = RequireService<IProvideValueTarget>(serviceProvider);
            imgTarget = ipvt.TargetObject as BitmapImage;
            propUriSource = ipvt.TargetProperty as DependencyProperty;
            if (imgTarget == null || propUriSource == null || propUriSource.Name != nameof(BitmapImage.UriSource))
                throw new NotSupportedException("This MarkupExtension can only be set on UriSource property of BitmapImage.");
        }

        private static readonly PropertyInfo s_propInheritanceContext = typeof(DependencyObject).GetProperty(
                "InheritanceContext", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly Func<DependencyObject, DependencyObject> s_funcGetInheritanceContext =
            (Func<DependencyObject, DependencyObject>)s_propInheritanceContext.GetGetMethod(true)
            .CreateDelegate(typeof(Func<DependencyObject, DependencyObject>));

        private static DependencyObject FindMentor(DependencyObject d)
        {
            while (d != null)
            {
                if (d.TryDowncastToFEorFCE(out FrameworkElement fe, out FrameworkContentElement fce))
                    return (DependencyObject)fe ?? fce;

                d = s_funcGetInheritanceContext(d);
            }
            return null;
        }

        private static DependencyObject GetParent(DependencyObject element)
        {
            Visual visual = element as Visual;
            DependencyObject parent = (visual == null) ? null : VisualTreeHelper.GetParent(visual);

            if (parent == null)
            {
                // No Visual parent. Check in the logical tree.
                parent = LogicalTreeHelper.GetParent(element);

                if (parent == null)
                {
                    if (element.TryDowncastToFEorFCE(out FrameworkElement fe, out FrameworkContentElement fce))
                        parent = fe != null ? fe.TemplatedParent : fce.TemplatedParent;
                }
            }

            if (parent == null)
                parent = FindMentor(element);

            return parent;
        }

        private static void SetInitialUriSource(BitmapImage img, Uri uri)
        {
            img.BeginInit();
            try
            {
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.UriSource = uri;
            }
            finally
            {
                img.EndInit();
            }
        }

        #region Package parts helpers
        //HINT: Package part URI helper methods are not public API, so use our own
        //implementation here, see: System.Windows.Navigation.BaseUriHelper

        private static bool IsPackApplicationUri(Uri uri)
        {
            if (uri.IsAbsoluteUri && string.Compare(uri.Scheme, PackUriHelper.UriSchemePack,
                StringComparison.OrdinalIgnoreCase) == 0)
            {
                return string.Compare(
                    PackUriHelper.GetPackageUri(uri).GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped),
                    _packageApplicationBaseUriEscaped,
                    StringComparison.OrdinalIgnoreCase) == 0;
            }
            return false;
        }

        private static void GetAssemblyNameAndPart(Uri uri, out string partName, out string assemblyName, out string assemblyVersion, out string assemblyKey)
        {
            if (uri == null || uri.IsAbsoluteUri)
                throw new ArgumentException("This method accepts relative uri only.", nameof(uri));

            string uriString = uri.ToString();
            int segmentStart = 0;
            if (uriString[0] == '/')
            {
                segmentStart = 1;
            }
            partName = uriString.Substring(segmentStart);
            assemblyName = string.Empty;
            assemblyVersion = string.Empty;
            assemblyKey = string.Empty;
            int segmentEnd = uriString.IndexOf('/', segmentStart);
            string firstSegment = string.Empty;
            bool flag = false;
            if (segmentEnd > 0)
            {
                firstSegment = uriString.Substring(segmentStart, segmentEnd - segmentStart);
                if (firstSegment.EndsWith(";component", StringComparison.OrdinalIgnoreCase))
                {
                    partName = uriString.Substring(segmentEnd + 1);
                    flag = true;
                }
            }
            if (!flag)
            {
                return;
            }
            string[] assemblyParts = firstSegment.Split(';');
            int cntAssemblyParts = assemblyParts.Length;
            if (cntAssemblyParts > 4 || cntAssemblyParts < 2)
            {
                throw new UriFormatException(WrongFirstSegment);
            }
            assemblyName = Uri.UnescapeDataString(assemblyParts[0]);
            for (int i = 1; i < cntAssemblyParts - 1; i++)
            {
                if (assemblyParts[i].StartsWith("v", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(assemblyVersion))
                    {
                        throw new UriFormatException(WrongFirstSegment);
                    }
                    assemblyVersion = assemblyParts[i].Substring(1);
                }
                else
                {
                    if (!string.IsNullOrEmpty(assemblyKey))
                    {
                        throw new UriFormatException(WrongFirstSegment);
                    }
                    assemblyKey = assemblyParts[i];
                }
            }
        }
        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Uri uriSource = UriSource;
            Uri partUri;
            if (uriSource.IsAbsoluteUri)
            {
                if (IsPackApplicationUri(uriSource))
                {
                    partUri = PackUriHelper.GetPartUri(uriSource);
                }
                else
                {
                    if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                        throw new InvalidOperationException("The Uri source must be a relative Uri.");

                    //internal Designer objects not always available and may change in future, so use safe reflection here
                    object viewNode = GetViewNodeFromSP(serviceProvider);
                    if (viewNode != null)
                    {
                        string docPath = GetDocumentPathFromDesignerObject(viewNode);
                        string docDir = Path.GetDirectoryName(docPath) ?? Path.GetPathRoot(docPath);
                        if (!uriSource.LocalPath.StartsWith(docDir))
                            throw new InvalidOperationException($"Cannot determine the relative URI for the absolute URI {uriSource} and the current Xaml document path {docPath}.");
                        string relativePath = uriSource.LocalPath.Substring(docDir.Length + 1);
                        uriSource = new Uri(relativePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), UriKind.Relative);
                        partUri = PackUriHelper.CreatePartUri(uriSource);
                    }
                    else
                    {
                        EnsureProvideValueTarget(serviceProvider, out var imgTarget, out var propUriSource);
                        SetInitialUriSource(imgTarget, uriSource);
                        return uriSource;
                    }
                }
            }
            else
            {
                partUri = PackUriHelper.CreatePartUri(uriSource);
            }

            GetAssemblyNameAndPart(partUri, out var partName, out var assemblyName, out _, out _);
            if (string.IsNullOrEmpty(partName))
                throw new NotSupportedException($"This MarkupExtension needs a relative package part URI to work with. URI was: {UriToString(uriSource)}");

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                uriSource = new Uri("/" + AssemblyName + COMPONENT + "/" + partName, UriKind.Relative);
                EnsureProvideValueTarget(serviceProvider, out var imgTarget, out var propUriSource);
                object viewNode = GetViewNodeFromSP(serviceProvider);
                if (viewNode != null)
                {
                    Uri absoluteSource = MakeDesignTimeUri(viewNode, uriSource);
                    if (BaseUri == null)
                        BaseUri = new Uri(GetDocumentPathFromDesignerObject(viewNode), UriKind.RelativeOrAbsolute);
                    SetInitialUriSource(imgTarget, absoluteSource);
                    return absoluteSource;
                }
                else
                {
                    var dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
                    if (dispatcher == null)
                        throw new NotSupportedException("This MarkupExtension needs to be used from the same thread as where Wpf designer is working on.");
                    DispatcherOperationCallback op = arg =>
                    {
                        var (img, uri) = ((BitmapImage, Uri))arg;
                        DependencyObject parent = img;
                        object context = null;
                        while (parent != null)
                        {
                            context = parent.GetValue(InstanceBuilderContextProperty);
                            if (context != null)
                                break;
                            parent = GetParent(parent);
                        }
                        context = UnwrapInstanceBuilderContext(context);
                        if (context == null)
                            throw new NotSupportedException("Designer object not found.");
                        Uri absoluteSource = MakeDesignTimeUri(context, uri);
                        SetInitialUriSource(img, absoluteSource);
                        return null;
                    };
#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
                    var dispop = dispatcher.BeginInvoke(op, DispatcherPriority.Normal, (imgTarget, uriSource));
#pragma warning restore VSTHRD001 // Avoid legacy thread switching APIs
                    if (BaseUri == null)
                        BaseUri = PackAppBaseUri;
                    return new Uri(BaseUri, uriSource);
                }
            }
            else
            {
                //HINT: the actual resource name will not include the project folder name of shared project,
                //so remove it here.
                if (partName.StartsWith(SharedRootNamespace + "/"))
                    partName = partName.Substring((SharedRootNamespace + "/").Length);
                uriSource = new Uri("/" + AssemblyName + COMPONENT + "/" + partName, UriKind.Relative);
                var uriContext = RequireService<IUriContext>(serviceProvider);
                //System.Windows.Navigation.BaseUriHelper.GetBaseUri(new System.Windows.DependencyObject())
                if (BaseUri == null)
                    BaseUri = uriContext.BaseUri;
            }

            return uriSource;
        }

        internal class SharedImageSourceExtensionConverter : TypeConverter
        {

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string) || sourceType == typeof(Uri) ||
                    base.CanConvertFrom(context, sourceType);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string) ||
                    destinationType == typeof(Uri) || base.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if ((value is string uriString && !string.IsNullOrEmpty(uriString)) || value is Uri)
                {
                    var (baseUri, originalUri) = GetUriFromUriContext(context, value);
                    return new SharedImageSourceExtension(originalUri) { BaseUri = baseUri };
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                var extension = value as SharedImageSourceExtension;
                if (extension != null)
                {
                    Uri uri = extension.BaseUri != null ? new Uri(extension.BaseUri, extension.UriSource) : extension.UriSource;
                    if (destinationType == typeof(InstanceDescriptor))
                    {
                        ConstructorInfo ctor = typeof(SharedImageSourceExtension).GetConstructor(new Type[1] { typeof(Uri) });
                        if (ctor == null)
                            throw new InvalidOperationException("The MarkupExtension type needs a constructor, which accepts a parameter of type Uri.");
                        return new InstanceDescriptor(ctor, new object[1] { uri });
                    }
                    else if (destinationType == typeof(string))
                    {
                        return UriToString(uri);
                    }
                    else if (destinationType == typeof(Uri))
                    {
                        return uri;
                    }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }

            private static (Uri BaseUri, Uri OriginalUri) GetUriFromUriContext(ITypeDescriptorContext context, object inputString)
            {
                Uri baseUri = null;
                Uri originalUri = null;
                if (inputString is string)
                {
                    originalUri = new Uri((string)inputString, UriKind.RelativeOrAbsolute);
                }
                else
                {
                    originalUri = (Uri)inputString;
                }
                if (!originalUri.IsAbsoluteUri && context != null)
                {
                    IUriContext uriContext = (IUriContext)context.GetService(typeof(IUriContext));
                    if (uriContext != null)
                    {
                        if (uriContext.BaseUri != (Uri)null)
                        {
                            baseUri = uriContext.BaseUri;
                            if (!baseUri.IsAbsoluteUri)
                            {
                                //System.Windows.Navigation.BaseUriHelper.GetBaseUri(new System.Windows.DependencyObject());
                                baseUri = new Uri(PackAppBaseUri, baseUri);
                            }
                        }
                        else
                        {
                            baseUri = PackAppBaseUri;
                        }
                    }
                }
                return (baseUri, originalUri);
            }
        }
    }
}
