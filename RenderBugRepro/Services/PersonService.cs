using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Bogus;
using DynamicData;
using ReactiveUI;
using RenderBugRepro.Models;
using Person = RenderBugRepro.Models.Person;

namespace RenderBugRepro.Services;

public class PersonService : IDisposable
{
    private readonly SourceCache<Person, int> _personCache;
    private readonly CompositeDisposable _disposables;
    private readonly ReadOnlyObservableCollection<Person> _people;

    public ReadOnlyObservableCollection<Person> People => _people;

    public PersonService()
    {
        _personCache = new SourceCache<Person, int>(person => person.Id);
        _disposables = new CompositeDisposable();

        // Connect the cache to a ReadOnlyObservableCollection
        _personCache
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _people)
            .Subscribe()
            .DisposeWith(_disposables);

        // Generate initial data
        GenerateInitialData();
    }

    private void GenerateInitialData()
    {
        var faker = new Faker<Person>()
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

        var people = faker.Generate(1000);
        _personCache.AddOrUpdate(people);
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
