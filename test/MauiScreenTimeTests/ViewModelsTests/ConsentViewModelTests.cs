using MauiScreenTime.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiScreenTimeTests.ViewModelsTests
{
    internal class ConsentViewModelTests
    {
        
        //public testable attributes:
        //HasConsent
        //public ICommand GrantConsentCommand
        //public ICommand RevokeConsentCommand
        //public ICommand DeleteAllCommand


        //test: HasConsentReturnsABool
        //Arrange
        //instantiate vm
        //Act
        //call HasConsent into a consent bool variable
        //Assert
        //assert consent is either true or false

        //test: GrantConsentCommandSetsConsentToTrue
        //Arrange
        //instantiate vm
        //Act
        //call GrantConsentCommand
        //Assert
        //assert HasConsent is equal to true

        //test: RevokeConsentCommandRemovesConsent
        //Arrange
        //instantiate vm
        //Act
        //call GrantConsentCommand to make sure HasConsent is set to true
        //call RevokeConsentCommand
        //Assert
        //assert HasConsent is equal to false

        //test: DeleteAllCommandRemovesAllConsents
        //Arrange
        //instantiate vm
        //Act
        //call GrantConsentCommand to make sure HasConsent is set to true
        //call DeleteAllCommand
        //Assert
        //assert HasConsent is equal to false

        //This are the only public testable methods and attributes. In order to test this the ConsentViewModel will need
        //to be refactored so that it does not directly depend on an instance of the sqlite db.
        //By adding an abstraction (via an interface) for the db we can use dependency injection to mock a db class that
        //would return the expected values when specific methods are called

    }



}
