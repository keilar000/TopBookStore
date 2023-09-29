﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TopBookStore.Application.DTOs;
using TopBookStore.Domain.Entities;
using TopBookStore.Domain.Interfaces;
using TopBookStore.Domain.Queries;
using TopBookStore.Infrastructure.Persistence;
using TopBookStore.Infrastructure.UnitOfWork;
using TopBookStore.Mvc.Grid;
using TopBookStore.Mvc.Models;
using TopBookStore.Mvc.Sessions;

namespace TopBookStore.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITopBookStoreUnitOfWork _data;

    public HomeController(ILogger<HomeController> logger, TopBookStoreContext context)
    {
        _logger = logger;
        _data = new TopBookStoreUnitOfWork(context);
    }

    public async Task<ViewResult> Index(GridDTO values, string? id)
    {
        GridBuilder builber = new(HttpContext.Session, values);

        QueryOptions<Book> options = new()
        {
            Includes = "Authors, Category",
            PageSize = builber.CurrentRoute.PageSize,
            PageNumber = builber.CurrentRoute.PageNumber
        };

        if (id is not null && id != string.Empty)
        {
            options.Where = b => b.CategoryId == id;
        }

        HomeIndexViewModel vm = new()
        {
            Books = await _data.Books.ListAllAsync(options),
            CurrentRoute = builber.CurrentRoute,
            TotalPages = builber.GetTotalPages(_data.Books.Count),
            Id = id ?? string.Empty
        };

        return View(vm);
    }

    public ViewResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
