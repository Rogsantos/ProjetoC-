﻿using Alura.ListaLeitura.App.Negocio;
using Alura.ListaLeitura.App.Repositorio;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.App
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }
        public void Configure(IApplicationBuilder app)
        {
            var builder = new RouteBuilder(app);
            builder.MapRoute("Livros/ParaLer", LivrosParaLer);
            builder.MapRoute("Livros/Lendo", LivrosLendo);
            builder.MapRoute("Livros/Lidos", LivrosLidos);
            builder.MapRoute("Cadastro/NovoLivro/{nome}/{autor}", NovoLivroParaLer);
            builder.MapRoute("Livros/Detalhes/{id:int}", ExibeDetalhes);
            builder.MapRoute("Cadastro/NovoLivro", ExibeFormulario);
            builder.MapRoute("Cadastro/Incluir", ProcessaFormulario);
            var rotas = builder.Build();
            app.UseRouter(rotas);
            //app.Run(Roteamento);
        }

        public Task ProcessaFormulario(HttpContext context)
        {
            var livro = new Livro()
            {
                Titulo = context.Request.Query["titulo"].First(),
                Autor = context.Request.Query["autor"].First()
            };
            var repo = new LivroRepositorioCSV();
            repo.Incluir(livro);
            return context.Response.WriteAsync("O livro foi adicionado com sucesso");
        }

        public Task ExibeFormulario(HttpContext context)
        {
            var html = @"
                        <html>
                        <form action='/Cadastro/Incluir'>
                            <label>Titulo: </label>
                            <input name='titulo'/>
                            <br/>
                            <label>Autor: </label>
                            <input name='autor'/>
                            <br/>
                            <button>Gravar</button>
                        </form>
                        </html>";
            return context.Response.WriteAsync(html);
        }

        public Task ExibeDetalhes(HttpContext context)
        {
            int id = Convert.ToInt32(context.GetRouteValue("id"));
            var repo = new LivroRepositorioCSV();
            var livro = repo.Todos.First(l =>  l.Id == id);
            return context.Response.WriteAsync(livro.Detalhes());
        }

        public Task NovoLivroParaLer(HttpContext context)
        {
            var livro = new Livro()
            {
                Titulo = Convert.ToString(context.GetRouteValue("nome")),
                Autor = Convert.ToString(context.GetRouteValue("autor"))
            };
            var repo = new LivroRepositorioCSV();
            repo.Incluir(livro);
            return context.Response.WriteAsync("O livro foi adicionado com sucesso");
        }

        public Task Roteamento(HttpContext context)
        {
            var _repor = new LivroRepositorioCSV();
            //Livros/ParaLer
            //Livros/Lendo
            //Livros/Lidos
            var caminhoAtendidos = new Dictionary<string, RequestDelegate>
            {
                {"/Livros/ParaLer", LivrosParaLer},
                {"/Livros/Lendo", LivrosLendo},
                {"/Livros/Lidos", LivrosLidos}
            };

            if (caminhoAtendidos.ContainsKey(context.Request.Path))
            {
                var metodo = caminhoAtendidos[context.Request.Path];
                return metodo.Invoke(context);
            }
            context.Response.StatusCode = 404;
            return context.Response.WriteAsync("Caminho inexistente");
        }
        public Task LivrosParaLer(HttpContext context)
        {
            var _repor = new LivroRepositorioCSV();
            return context.Response.WriteAsync(_repor.ParaLer.ToString());
        }
        public Task LivrosLendo(HttpContext context)
        {
            var _repor = new LivroRepositorioCSV();
            return context.Response.WriteAsync(_repor.Lidos.ToString());
        }
        public Task LivrosLidos(HttpContext context)
        {
            var _repor = new LivroRepositorioCSV();
            return context.Response.WriteAsync(_repor.Lidos.ToString());
        }
    }
}