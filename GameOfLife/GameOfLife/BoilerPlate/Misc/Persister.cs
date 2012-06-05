using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using GameOfLife.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace GameOfLife.BoilerPlate.Misc {
    public static class Persister {
        private static StorageDevice _storageDevice;

        public static void DepdencyInjection(StorageDevice storageDevice) {
            _storageDevice = storageDevice;
        }

        public static void LoadFromLoose<T>(string path, Action<T> userCallback) where T : class {
            try {
                using (var fs = TitleContainer.OpenStream(path)) {
                    using (
                        var reader = XmlDictionaryReader.CreateTextReader(fs,
                                                                          new XmlDictionaryReaderQuotas { MaxDepth = Int32.MaxValue })) {
                        var serializer = new NetDataContractSerializer();

                        T loadedObject = null;
                        loadedObject = serializer.ReadObject(reader, true) as T;
                        userCallback(loadedObject);
                    }
                }
            } catch (Exception e) {
                Console.WriteLine("Failed loading {0} from asset. {1}", path, e);
            }

        }

        public static void SaveToDevice<T>(T obj, String path, Action userCallback) where T : class {
            if (_storageDevice == null) {
                Console.WriteLine("Storage Device was null");
                if (userCallback != null) userCallback();
                return;
            }
            _storageDevice.BeginOpenContainer(Constants.Locations.ContainerName, openResult => {
                using (var sd = _storageDevice.EndOpenContainer(openResult)) {

                    var serializer = new NetDataContractSerializer();
                    var writerSettings = new XmlWriterSettings { Indent = true, CloseOutput = true };

                    using (var writer = XmlWriter.Create(sd.CreateFile(path), writerSettings)) {
                        serializer.WriteObject(writer, obj);
                    }

                    if (userCallback != null) userCallback();
                }

            }, null);
        }

        public static void LoadFromDevice<T>(String path, Action<T> userCallback) where T : class {
            if (_storageDevice == null) {
                Console.WriteLine("Storage Device was null");
                userCallback(null);
                return;
            }

            _storageDevice.BeginOpenContainer(Constants.Locations.ContainerName, openResult => {
                using (var sd = _storageDevice.EndOpenContainer(openResult)) {
                    if (!sd.FileExists(path)) {
                        userCallback(null);
                        return;
                    }

                    var serializer = new NetDataContractSerializer();
                    using (var fs = sd.OpenFile(path, FileMode.Open)) {
                        using (var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas { MaxDepth = Int32.MaxValue })) {
                            T loadedObject = null;
                            try {
                                loadedObject = serializer.ReadObject(reader, true) as T;
                            } catch (Exception e) {
                                Console.WriteLine("Failed loading {0} from asset. {1}", path, e.StackTrace);
                            }
                            userCallback(loadedObject);
                        }
                    }
                }
            }, null);
        }

        public static void LoadAllFileNames(String searchPattern, Action<String[]> userCallback) {
            if (_storageDevice == null) {
                Console.WriteLine("Storage Device was null");
                userCallback(null);
                return;
            }

            _storageDevice.BeginOpenContainer(Constants.Locations.ContainerName, openResult => {
                using (var sd = _storageDevice.EndOpenContainer(openResult)) {
                    var fileNames = sd.GetFileNames(searchPattern);
                    userCallback(fileNames);
                }
            }, null);
        }
    }
}


