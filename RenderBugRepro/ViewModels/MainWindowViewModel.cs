using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using RenderBugRepro.Models;
using RenderBugRepro.Services;

namespace RenderBugRepro.ViewModels;

public class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly PersonService _personService;
    
    public ReadOnlyObservableCollection<Person> People => _personService.People;
    
    public FlatTreeDataGridSource<Person> TreeDataGridSource { get; }

    public MainWindowViewModel()
    {
        _personService = new PersonService();
        
        TreeDataGridSource = new FlatTreeDataGridSource<Person>(People)
        {
            Columns =
            {
                new TextColumn<Person, int>("ID", x => x.Id, width: new GridLength(60)),
                new TextColumn<Person, string>("First Name", x => x.FirstName, width: new GridLength(120)),
                new TextColumn<Person, string>("Last Name", x => x.LastName, width: new GridLength(120)),
                new TextColumn<Person, string>("Email", x => x.Email, width: new GridLength(200)),
                new TextColumn<Person, string>("Phone", x => x.PhoneNumber, width: new GridLength(140)),
                new TextColumn<Person, DateTime>("Date of Birth", x => x.DateOfBirth, width: new GridLength(120)),
                new TextColumn<Person, int>("Age", x => x.Age, width: new GridLength(60)),
                new TextColumn<Person, string>("Address", x => x.Address, width: new GridLength(200)),
                new TextColumn<Person, string>("City", x => x.City, width: new GridLength(120)),
                new TextColumn<Person, string>("Country", x => x.Country, width: new GridLength(120)),
                new TextColumn<Person, string>("Company", x => x.Company, width: new GridLength(150)),
                new TextColumn<Person, string>("Job Title", x => x.JobTitle, width: new GridLength(150)),
                new TextColumn<Person, decimal>("Salary", x => x.Salary, width: new GridLength(100))
            }
        };
    }

    public void Dispose()
    {
        _personService?.Dispose();
    }
}