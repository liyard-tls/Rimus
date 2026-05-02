using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Rimus.Scripts.Tools
{
    public enum LogType
    {
        Info,
        Warning,
        Error,
        SendToServer,
        ServerResponse,
        Temporary,
        ClientInput,
        BattleLog,
        Application,
        WSClient,
        Analytics,
        PassCheck,
        ByType
    }
    
    public static class Log
    {
        public static Dictionary<LogType, string> LogTypeColors = new Dictionary<LogType, string>
        {
            {LogType.Info, "#808080"},      
            {LogType.Warning, "#FFA500"},
            {LogType.Error, "#DC143C"},    
            {LogType.SendToServer, "#32CD32"}, 
            {LogType.ServerResponse, "#FFD700"}, 
            {LogType.Temporary, "#FC03F8"},
            {LogType.ClientInput, "#1E90FF"},
            {LogType.BattleLog, "#00CED1"},
            {LogType.Application, "#4287f5"},
            {LogType.WSClient, "#FF69B4"},
            {LogType.Analytics, "#FF4500"},
            {LogType.PassCheck, "#00FF00"}, 
            {LogType.ByType, "#00BFFF"},
        };
        
        public static bool IsLogsEnabled { get; private set; }
        
        public static Dictionary<LogType, bool> LogTypeEnabled = new Dictionary<LogType, bool>
        {
            {LogType.Info, true},
            {LogType.Warning, true},
            {LogType.SendToServer, true},
            {LogType.ServerResponse, true},
            {LogType.Error, true},
            {LogType.Temporary, true},
            {LogType.ClientInput, true},
            {LogType.BattleLog, true},
            {LogType.Application, true},
            {LogType.WSClient, true},
            {LogType.Analytics, true},
            {LogType.PassCheck, true},
            {LogType.ByType, true},
        };
        
        private static Dictionary<LogType, bool> _releaseLogTypeEnabled = new Dictionary<LogType, bool>
        {
            {LogType.Info, false},
            {LogType.Warning, true},
            {LogType.SendToServer, true},
            {LogType.ServerResponse, true},
            {LogType.Error, true},
            {LogType.Temporary, false},
            {LogType.ClientInput, false},
            {LogType.BattleLog, false},
            {LogType.Application, true},
            {LogType.WSClient, true},
            {LogType.Analytics, true},
            {LogType.PassCheck, false},
            {LogType.ByType, false},
        };
        
        private static Dictionary<LogType, bool> _debugLogTypeEnabled = new Dictionary<LogType, bool>
        {
            {LogType.Info, true},
            {LogType.Warning, true},
            {LogType.SendToServer, true},
            {LogType.ServerResponse, true},
            {LogType.Error, true},
            {LogType.Temporary, true},
            {LogType.ClientInput, true},
            {LogType.BattleLog, true},
            {LogType.Application, true},
            {LogType.WSClient, true},
            {LogType.Analytics, true},
            {LogType.PassCheck, true},
            {LogType.ByType, true},
        };
        
        public static void Initialize()
        {
            //#if UNITY_EDITOR || DEVELOPMENT_BUILD
                IsLogsEnabled = true;
                LogTypeEnabled = new Dictionary<LogType, bool>(_debugLogTypeEnabled);
            // #else
            //     IsLogsEnabled = false;
            //     LogTypeEnabled = new Dictionary<LogType, bool>(_releaseLogTypeEnabled);
            // #endif
        }
        
        public static void SetLogsEnabled(bool enabled)
        {
            IsLogsEnabled = enabled;
            if (enabled)
            {
                LogTypeEnabled = new Dictionary<LogType, bool>(_debugLogTypeEnabled);
            }
            else
            {
                LogTypeEnabled = new Dictionary<LogType, bool>(_releaseLogTypeEnabled);
            }
        }
        
        public static void LogMessage(LogType logType, string message, Exception exception = null)
        {
            if (!LogTypeEnabled[logType]) return;
            
            string color = LogTypeColors[logType];
            string logMessage = $"<color={color}>[{logType}]: {message}</color>";

            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError(logMessage);
                    break;
                    if (exception != null)
                    {
                        exception.Data.Add("LogMessage", logMessage);
                        Debug.LogException(exception);
                    }
                    else
                    {
                        Debug.LogError(logMessage);
                    }
                    break;
                default:
                    Debug.Log(logMessage);
                    break;
            }
        }
        
        public static void LogObject(object obj, string label = null, bool saveInFile = false, bool saveInFileOnOverflow = false, bool indent = true)
        {
            if (obj == null)
            {
                Debug.LogWarning("LogObject: null");
                return;
            }

            string output;

            try
            {
                output = JsonConvert.SerializeObject(obj, indent ? Formatting.Indented : Formatting.None);
                if (saveInFile || (output.Length > 10000 && saveInFileOnOverflow))
                {
                    string objType = obj.GetType().Name;
                    string fileName = $"{objType}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                    System.IO.File.WriteAllText(fileName, output);
                }
                
            }
            catch (Exception e)
            {
                Debug.LogError("JsonConvert failed: " + e.Message);
                output = obj.ToString();
            }
            Debug.Log(string.IsNullOrEmpty(label) ? output : $"{label}:\n{output}");
        }
        
        public static string GetString(object obj, bool indent = true)
        {
            if (obj == null)
            {
                return "LogObject: null";
            }

            string output;

            try
            {
                output = JsonConvert.SerializeObject(obj, indent ? Formatting.Indented : Formatting.None);
            }
            catch (Exception e)
            {
                Debug.LogError("JsonConvert failed: " + e.Message);
                output = obj.ToString();
            }
            return output;
        }
        
        public static void LogByType<T>(T self, string message)
        {
            if (self == null)
            {
                Error($"Object of type {typeof(T).Name} is null");
                return;
            }
            
            string typeName = typeof(T).Name;
            string logMessage = $"[{typeName}] {message}";
            LogMessage(LogType.ByType, logMessage);
            
        }
        
        public static void Info(string message)
        {
            LogMessage(LogType.Info, message);
        }
        
        public static void Warning(string message)
        {
            LogMessage(LogType.Warning, message);
        }
        
        public static void Execution(string message)
        {
            LogMessage(LogType.SendToServer, message);
        }
        
        public static void Error(string message, Exception exception = null)
        {
            LogMessage(LogType.Error, $"{message}", exception);
        }
        
        public static void Temporary(string message)
        {
            LogMessage(LogType.Temporary, message);
        }
        
        public static void ClientInput(string message)
        {
            LogMessage(LogType.ClientInput, message);
        }
        
        public static void BattleLog(string message)
        {
            LogMessage(LogType.BattleLog, message);
        }
        
        public static void ServerResponse(string message)
        {
            LogMessage(LogType.ServerResponse, message);
        }

        public static void Analytics(string message)
        {
            LogMessage(LogType.Analytics, message);
        }

        public static void Application(string message)
        {
            LogMessage(LogType.Application, message);
        }
        
        public static void WSClient(string message)
        {
            LogMessage(LogType.WSClient, message);
        }

        public static void PassCheck(string message)
        {
            LogMessage(LogType.PassCheck, message);
        }

        public static void CheckNull(object obj, string label = null)
        {
            if (obj == null)
            {
                Error($"Object is null: {label ?? "Unknown"}");
            }
            else
            {
                Info($"Object is not null: {label ?? "Unknown"}");
            }
        }

        public static string ToJson(this object obj, bool indent = true)
        {
            return GetString(obj, indent);
        }

        public static void FastLog(this object obj)
        {
            if (obj == null)
            {
                Debug.Log("null");
                return;
            }

            try
            {
                string output = GetString(obj);
                string typeName = obj.GetType().Name;
                Temporary($"[{typeName}] {output}");
            }
            catch (Exception e)
            {
                Debug.LogError("JsonConvert failed: " + e.Message);
                Debug.Log(obj.ToString());
            }
        }

    }
}