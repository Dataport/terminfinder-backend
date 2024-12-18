# Terminfinder-Backend

The "Terminfinder" offers a fast and easy way to create digital polls.  

The "Terminfinder" (frontend, backend, database) allows you to quickly and easily create polls for appointments.  
Simply select a title for your poll and, if required, the location where the appointment should take place.  
Afterwards you can directly get started and share the link to your poll with those you want to share it with.  
With the help of the "Terminfinder" you can flexibly decide whether you want to offer different days,
different times or a combination of both within your poll.  
For increased security it is also possible to set a password for your poll.  
The slim design of the "Terminfinder" ensures that you and the participants can easily navigate through the
application. In addition, the "Terminfinder" only collects necessary data which is not shared with third parties.  
After the last appointment option expires the data is automatically deleted.  
Thus, data is kept only as long as necessary.  

features:  
* Create appointment polls and put one or more options to the vote
* Participate in appointment polls and respond to the appointment suggestions
* Choices for responding - "Accept, Decline or Questionable"

Licensed under the EUPL 1.2

Copyright © 2022-2023 Dataport AöR

[SECURITY.md](docs/SECURITY.md)

[CONTRIBUTING.md](docs/CONTRIBUTING.md)

## Parts of the backend

* WebAPI-Application
* Service in order to delete data after last appointment option expired
* PostgreSql Database

## Requirements
* Microsoft dotnet 8, [ASP.NET Core Runtime 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* Database [PostgreSQL](https://www.postgresql.org/)

## Database
First, the database has to be created by yourself. Afterwards the `additional supplied module` `uuid-ossp` has to be installed because the database generates unique uuids.  
When starting the webapi-application with the parameter `--dbmigrate` the tables are being created in the database and a first customer with the customerid `80248A42-8FE2-4D4A-89DA-02E683511F76` will be inserted.  

## Configuration
The connection-string has to be defined in the `appsettings.json`.  

In the production deployment we configure cors and ssl/tls via the ingress controller, so `UseHsts`, `UseHttpsRedirect` and `UseCors` are disabled.
You can enable it by configuration in the `appsettings.json`.
```
"Terminfinder:UseHttps:": "true"
"Terminfinder:UseCors:": "true"
```

All configurations can by set by environments, for example:
```
set Terminfinder__UseCors=true
set Terminfinder__UseHttps=true
set ConnectionStrings__TerminfinderConnection=Server=127.0.0.1;Port=5432;Database=terminfinder;User ID=user;password=pw;
```

The Customers have an unique uuid. In the database the customers are stored in den table `customer`. The frontend communicates with the backend by using this `customerid`.  
```
insert into public.customer (customerid, customername, status) values ('GUID', 'Customername', 'Started');
```

## Service to delete expired data (Dataport.Terminfinder.DeleteAppointments.Tool)
The service to delete expired data can be found in the directory `Dataport.Terminfinder.DeleteAppointments.Tool`.  

### how to start the application
to call the application:  
```
dotnet Dataport.Terminfinder.DeleteAppointments.dll customerId 'days after expired appointments should be deleted'
```
**sample**  
```
dotnet Dataport.Terminfinder.DeleteAppointments.dll "5C075919-0374-4063-A2C7-3147C6A22C30" 7
```
