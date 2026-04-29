using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rimus.Scripts.Tools.GameContentSystem
{
    public static class GameContentDB
    {
        private static List<ContentDatabase> databases = new List<ContentDatabase>();

        public static void RegisterDatabase(ContentDatabase database)
        {
            databases ??= new List<ContentDatabase>();
            if (!databases.Contains(database))
                databases.Add(database);
        }

        public static void UnregisterDatabase(ContentDatabase database)
        {
            databases?.Remove(database);
        }

        public static T GetContentById<T>(string id) where T : IGameContent
        {
            foreach (var database in databases)
            {
                foreach (var content in database.Content)
                {
                    if (content is T typedContent && typedContent.Id == id)
                    {
                        return typedContent;
                    }
                }
            }
            throw new Exception($"Content with ID '{id}' not found.");
        }
    }
}