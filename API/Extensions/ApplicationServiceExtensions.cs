using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        //TODO:how to use this in csharp
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config){
            //different lifetime
            //AddSingleTon(existed until the app stop), AddTransient(existed until the method finished)
            //AddScoped (Scoped through a HTTP request)
            services.AddScoped<ITokenService, Tokenservice>();
            services.AddDbContext<DataContext>(options =>{
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}