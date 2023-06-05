using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;

using Microsoft.VisualStudio.ConnectedServices;

namespace Unchase.OpenAPI.ConnectedService.Common
{
    internal static class UserSettingsPersistenceHelper
    {
        #region Public methods

        /// <summary>
        /// Saves user settings to isolated storage.  The data is stored with the user's roaming profile.
        /// </summary>
        /// <remarks>
        /// Non-critical exceptions are handled by writing an error message in the output window.
        /// </remarks>
        public static async Task SaveAsync(
            object userSettings,
            string providerId,
            string name,
            Action onSaved,
            ConnectedServiceLogger logger)
        {
            var fileName = GetStorageFileName(providerId, name);

            await ExecuteNoncriticalOperationAsync(
                async () =>
                {
                    using (var file = GetIsolatedStorageFile())
                    {
                        IsolatedStorageFileStream stream = null;
                        try
                        {
                            // note: this overwrites existing settings file if it exists
                            stream = file.OpenFile(fileName, FileMode.Create);
                            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Async = true }))
                            {
                                stream = null;

                                //TODO: use async serializer
                                Type[] knownTypes = new[]
                                {
                                    typeof(Microsoft.OpenApi.OData.Extensions.ODataRoutePathPrefixProvider)
                                };
                                var dcs = new DataContractSerializer(userSettings.GetType(), knownTypes);
                                dcs.WriteObject(writer, userSettings);

                                await writer.FlushAsync();
                            }
                        }
                        finally
                        {
                            stream?.Dispose();
                        }
                    }

                    onSaved?.Invoke();
                },
                logger,
                "Failed loading the {0} user settings",
                fileName);
        }

        /// <summary>
        /// Loads user settings from isolated storage.
        /// </summary>
        /// <remarks>
        /// Non-critical exceptions are handled by writing an error message in the output window and
        /// returning null.
        /// </remarks>
        public static async Task<T> LoadAsync<T>(
            string providerId,
            string name,
            Action<T> onLoaded,
            ConnectedServiceLogger logger)
            where T : class
        {
            var fileName = GetStorageFileName(providerId, name);
            T result = null;

            await ExecuteNoncriticalOperationAsync(
                async () =>
                {
                    using (var file = GetIsolatedStorageFile())
                    {
                        if (file.FileExists(fileName))
                        {
                            IsolatedStorageFileStream stream = null;
                            try
                            {
                                stream = file.OpenFile(fileName, FileMode.Open);
                                var settings = new XmlReaderSettings
                                {
                                    Async = true,
                                    XmlResolver = null
                                };

                                using (var reader = XmlReader.Create(stream, settings))
                                {
                                    stream = null;

                                    //TODO: use async serializer
                                    Type[] knownTypes = new[]
                                    {
                                        typeof(Microsoft.OpenApi.OData.Extensions.ODataRoutePathPrefixProvider)
                                    };
                                    var dcs = new DataContractSerializer(typeof(T), knownTypes);
                                    result = dcs.ReadObject(reader) as T;
                                }
                            }
                            finally
                            {
                                stream?.Dispose();
                            }

                            if (onLoaded != null && result != null)
                            {
                                onLoaded(result);
                            }
                        }
                    }
                },
                logger,
                "Failed loading the {0} user settings",
                fileName);

            return result;
        }

        #endregion

        #region Private methods

        private static string GetStorageFileName(string providerId, string name)
        {
            return providerId + "_" + name + ".xml";
        }

        private static IsolatedStorageFile GetIsolatedStorageFile()
        {
            return IsolatedStorageFile.GetStore(
                IsolatedStorageScope.Assembly | IsolatedStorageScope.User | IsolatedStorageScope.Roaming, null, null);
        }

        private static async Task ExecuteNoncriticalOperationAsync(
            Func<Task> operation,
            ConnectedServiceLogger logger,
            string failureMessage,
            string failureMessageArg)
        {
            try
            {
                await operation();
            }
            catch (Exception ex)
            {
                await logger.WriteMessageAsync(LoggerMessageCategory.Warning, failureMessage, failureMessageArg, ex);
            }
        }

        #endregion
    }
}