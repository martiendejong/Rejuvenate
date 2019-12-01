﻿using Microsoft.Owin;
using Owin;
using AutoMapper;
using CardGame.Models;

[assembly: OwinStartupAttribute(typeof(CardGame.Startup))]
namespace CardGame
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
