namespace FsMosquito.Services
{
    using FsMosquito.Models;
    using LiteDB;
    using Microsoft.Extensions.Options;
    using System;
    using System.IO;

    public interface ILiteDbContext
    {
        LiteDatabase FSMosquitoDB { get; }
    }

    public class LiteDbContext : ILiteDbContext
    {
        public LiteDatabase FSMosquitoDB { get; }

        public LiteDbContext(IOptions<LiteDbOptions> options)
        {
            var targetAppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "fsmosquito-desktop-data");
            if (!Directory.Exists(targetAppPath))
            {
                Directory.CreateDirectory(targetAppPath);
            }

            FSMosquitoDB = new LiteDatabase(Path.Combine(targetAppPath, options.Value.FSMosquitoDBLocation));
        }
    }
}
