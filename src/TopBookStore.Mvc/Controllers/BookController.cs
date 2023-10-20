using Microsoft.AspNetCore.Mvc;
using TopBookStore.Application.DTOs;
using TopBookStore.Domain.Entities;
using TopBookStore.Domain.Queries;
using TopBookStore.Mvc.Grid;
using TopBookStore.Mvc.Models;
using TopBookStore.Application.Interfaces;
using TopBookStore.Domain.Interfaces;

namespace TopBookStore.Mvc.Controllers;

public class BookController : Controller
{
    private readonly IBookService _service;
    private readonly ITopBookStoreUnitOfWork _data;

    public BookController(IBookService service, ITopBookStoreUnitOfWork data)
    {
        _service = service;
        _data = data;
    }

    public async Task<IActionResult> Index(GridDto values)
    {
        GridBuilder builder = new(HttpContext.Session, values);

        BookListViewModel vm = new()
        {
            Books = await _service.FilterBooksAsync(values),
            Categories = await _data.Categories.ListAllAsync(new QueryOptions<Category>
            {
                OrderBy = a => a.Name
            }),
            Authors = await _data.Authors.ListAllAsync(new QueryOptions<Author>
            {
                OrderBy = a => a.FirstName
            }),
            CurrentRoute = builder.CurrentRoute
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        Book? book = await _service.GetBookByIdAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        return View(book);
    }

    public RedirectToActionResult FilterBooks(string[] filter, bool clear = false)
    {
        // get current route segments from session
        GridBuilder builder = new(HttpContext.Session);

        // clear or update filter route segment values. If update, get author data
        // from database so can add author name slug to author filter value.
        if (clear)
        {
            builder.ClearFilterSegments();
        }
        else
        {
            builder.LoadFilterSegments(filter);
        }

        // save route data back to session and redirect to Book/List action method,
        // passing dictionary of route segment values to build URL
        builder.SaveRouteSegments();

        return RedirectToAction(nameof(Index), builder.CurrentRoute);
    }
}