using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;

namespace MinimalApiPeliculas.Enpoints
{
    public static class GenerosEndPoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            //Obtener Todos Los Generos
            group.MapGet("/", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("generos-get"));

            //Obtener Generos Por Id
            group.MapGet("/{id:int}", ObtenerGeneroPorId);

            //Crear Genero
            group.MapPost("/", CrearGenero);

            //Actualizar Genero
            group.MapPut("/{id:int}", Actualizargenero);

            //Borrar Genero
            group.MapDelete("/{id:int}", EliminarGenero);

            return group;
        }

        //******************FUNCIONES PARA LOS GENEROS*********************
        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio, IMapper mapper)
        {
            var genero = await repositorio.ObtenerTodos();

            var generoDTO = mapper.Map<List<GeneroDTO>>(genero);

            return TypedResults.Ok(generoDTO);
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId(int id, IRepositorioGeneros repositorio, IMapper mapper)
        {
            var genero = await repositorio.ObtenerPorId(id);

            if (genero is null)
            {
                return TypedResults.NotFound();
            }

            var generoDTO = mapper.Map<GeneroDTO>(genero); 

            return TypedResults.Ok(generoDTO);

        }

        static async Task<Created<GeneroDTO>> CrearGenero(CrearGeneroDTO crearGeneroDTOgenero, IRepositorioGeneros repositorioGeneros, IOutputCacheStore cacheStore, IMapper mapper)
        {
            var genero = mapper.Map<Genero>(crearGeneroDTOgenero);
            var id = await repositorioGeneros.CrearGenero(genero);
            await cacheStore.EvictByTagAsync("generos-get", default);

            var generoDTO = mapper.Map<GeneroDTO>(genero);

            return TypedResults.Created($"/generos/{id}", generoDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizargenero(int id, CrearGeneroDTO crearGeneroDTOgenero, IRepositorioGeneros repositorio, IOutputCacheStore cacheStore, IMapper mapper)
        {
            var existe = await repositorio.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }

            var genero = mapper.Map<Genero>(crearGeneroDTOgenero);
            genero.Id = id;

            await repositorio.Actualizar(genero);
            await cacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> EliminarGenero(int id, IRepositorioGeneros repositorio, IOutputCacheStore cacheStore)
        {
            var existe = await repositorio.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await cacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

    }
}