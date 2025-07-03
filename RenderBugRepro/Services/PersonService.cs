using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Bogus;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using RenderBugRepro.Models;
using Person = RenderBugRepro.Models.Person;

namespace RenderBugRepro.Services;

public class PersonService : IDisposable
{
    private readonly SourceCache<Person, int> _personCache;
    private readonly CompositeDisposable _disposables;
    private readonly ReadOnlyObservableCollection<Person> _people;
    private readonly Faker<Person> _faker;
    private readonly Random _random;

    public ReadOnlyObservableCollection<Person> People => _people;

    public PersonService()
    {
        _personCache = new SourceCache<Person, int>(person => person.Id);
        _disposables = new CompositeDisposable();
        _random = new Random();

        // Setup the faker for updates
        _faker = new Faker<Person>()
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName())
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(p => p.DateOfBirth, f => f.Date.Between(DateTime.Now.AddYears(-65), DateTime.Now.AddYears(-18)))
            .RuleFor(p => p.Address, f => f.Address.FullAddress())
            .RuleFor(p => p.City, f => f.Address.City())
            .RuleFor(p => p.Country, f => f.Address.Country())
            .RuleFor(p => p.Company, f => f.Company.CompanyName())
            .RuleFor(p => p.JobTitle, f => f.Name.JobTitle())
            .RuleFor(p => p.Salary, f => f.Random.Decimal(30000, 150000));

        // Connect the cache to a ReadOnlyObservableCollection
        _personCache
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .SortAndBind(out _people, SortExpressionComparer<Person>.Ascending(p => p. LastName))
            // .Bind(out _people)
            .Subscribe()
            .DisposeWith(_disposables);

        // Generate initial data
        GenerateInitialData();

        // Start periodic updates
        StartPeriodicUpdates();
    }

    private void GenerateInitialData()
    {
        var initialFaker = new Faker<Person>()
            .RuleFor(p => p.Id, f => f.IndexFaker + 1)
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName())
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(p => p.DateOfBirth, f => f.Date.Between(DateTime.Now.AddYears(-65), DateTime.Now.AddYears(-18)))
            .RuleFor(p => p.Address, f => f.Address.FullAddress())
            .RuleFor(p => p.City, f => f.Address.City())
            .RuleFor(p => p.Country, f => f.Address.Country())
            .RuleFor(p => p.Company, f => f.Company.CompanyName())
            .RuleFor(p => p.JobTitle, f => f.Name.JobTitle())
            .RuleFor(p => p.Salary, f => f.Random.Decimal(30000, 150000));

        var people = initialFaker.Generate(5000);
        _personCache.AddOrUpdate(people);
    }

    private void StartPeriodicUpdates()
    {
        // Create a timer that triggers every 200ms
        Observable.Timer(TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(200))
            .ObserveOn(RxApp.TaskpoolScheduler) // Do the work off the UI thread
            .Subscribe(_ => UpdateRandomEntries())
            .DisposeWith(_disposables);
    }

    private void UpdateRandomEntries()
    {
        try
        {
            // Get current items from cache
            var currentItems = _personCache.Items.ToList();
            if (!currentItems.Any()) return;

            // Randomly select 200-500 entries to update
            var updateCount = _random.Next(500, 701); // 200 to 500 inclusive
            var indicesToUpdate = Enumerable.Range(0, currentItems.Count)
                .OrderBy(_ => _random.Next())
                .Take(updateCount)
                .ToList();

            var updatedPeople = new List<Person>();

            foreach (var index in indicesToUpdate)
            {
                var existingPerson = currentItems[index];

                // Create an updated person with the same ID but new data
                var updatedPerson = _faker.Generate();
                updatedPerson.Id = existingPerson.Id; // Keep the same ID

                updatedPeople.Add(updatedPerson);
            }

            // Update all selected entries at once
            if (updatedPeople.Any())
            {
                _personCache.AddOrUpdate(updatedPeople);
            }
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            Console.WriteLine($"Error updating entries: {ex.Message}");
        }
    }

    public void AddPerson(Person person)
    {
        _personCache.AddOrUpdate(person);
    }

    public void RemovePerson(int id)
    {
        _personCache.RemoveKey(id);
    }

    public void UpdatePerson(Person person)
    {
        _personCache.AddOrUpdate(person);
    }

    public void Dispose()
    {
        _disposables?.Dispose();
        _personCache?.Dispose();
    }
}
