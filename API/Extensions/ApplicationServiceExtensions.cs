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
        //usually static class cant use this, except extension method
        //what is extension method? 
        //It is a new approach or concept that has been added in C#3.0 version 
        //which allows to add new methods in an existing class without editing the source of the class.
 
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