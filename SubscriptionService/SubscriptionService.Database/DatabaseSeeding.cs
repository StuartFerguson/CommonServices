﻿using Microsoft.EntityFrameworkCore;

namespace SubscriptionService.Database
{
    public class DatabaseSeeding
    {
        /// <summary>
        /// Initialises the database.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void InitialiseDatabase(SubscriptionServiceConfigurationContext context)
        {
            if (context.Database.IsSqlServer())
            {
                context.Database.Migrate();
            }
        
            context.SaveChanges();
        }
    }
}
