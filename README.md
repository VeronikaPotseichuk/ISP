# Лабораторная работа №4

## Потейчук Вероника 953502

## Предисловие

Это всё та же Windows-служба, только с использованием базы данных AdventureWorks2019, которую посоветовал скачать Артём Петрович)

### Как работает:

* Наша служба (**DataManager**) запускается;
* Следующим этапом идёт сбор необходимых настроек из XML/JSON;
* Из базы данных *AventureWorks2019* извлекаются, как не удивительно, данные, на основе которых фосмируются XML и XSD файлы;
* Затем сформированные файлы отправляются на FTP (File Transfer Protocol) сервер, представленный в третьей лабораторной как SourceDirectory;
* Служба **FileManager** делает всю необходимую работу по отправке данных в TargetDirectory;
* Все успешные действия и исключения записываются в БД **ApplicationInsights** и когда работа завершается, то данные записываются в XML файл и создается XSD;
* Конец работы **DataManager**

    * Лирическое отступление: Службы **DataManager** и **FileMenager** работают одновременно и все настройки берут из **ConfigManager**.

## Реализация

Объединённые 5 таблиц, которые связаны между собой внешним ключом, выведены в таблицу покупателей в разных магазинах. Они отсортированы по их номеру счета (AccountNumber):

![Screenshot](Screenshots/1.png)


Запрос к базе данных:

```sql
select
Person.Person.FirstName,
Person.Persom.LastName,
Sales.Customer.AccountNumber,
Sales.Store.Name as StoreName,
Sales.SalesTerritory.CountryRegionCode,
Person.CountryRegion.Name,
Sales.CountryRegionCurrency.CurrencyCode,
Sales.Currency.Name,
from Sales.Customer
join Sales.Store on Sales.Customer.StoreID = Sales.Store.BusinessEntityID
join Sales.SalesTrritory on Sales.SalesTerritory.TerritoryID = Sales.Customer.TerritoryId
join Person.Person on Sales.Customer.PersonID = person.Person.BusinessEntityID
join Sales.CountryRegionCurrency on Sales.SalesTerritory.CountryRegionCode = Sales.CountryRegionCurrency.CountryRegionCode
join Sales.Currency on Sales.CountryRegionCurrency.CurrencyCode = Sales.Currency.CurrencyCode
join Person.CountryRegion on Sales.SalesTerritory.CountryRegionCode = Person.CountryRegion.CountryRegionCode
order by AccountNumber
```

Затем этот запрос стал основой хранимой процедуры sp_GetCustomers:

```sql
USE [AdventureWorks2019]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_iDENTIFIER ON
GO
ALTER procedure [dbo].[sp_GetCustomers]
as
    select
    Person.Person.FirstName,
    Person.Persom.LastName,
    Sales.Customer.AccountNumber,
    Sales.Store.Name as StoreName,
    Sales.SalesTerritory.CountryRegionCode,
    Person.CountryRegion.Name,
    Sales.CountryRegionCurrency.CurrencyCode,
    Sales.Currency.Name,
    from Sales.Customer
    join Sales.Store on Sales.Customer.StoreID = Sales.Store.BusinessEntityID
    join Sales.SalesTrritory on Sales.SalesTerritory.TerritoryID = Sales.Customer.TerritoryId
    join Person.Person on Sales.Customer.PersonID = person.Person.BusinessEntityID
    join Sales.CountryRegionCurrency on Sales.SalesTerritory.CountryRegionCode = Sales.CountryRegionCurrency.CountryRegionCode
    join Sales.Currency on Sales.CountryRegionCurrency.CurrencyCode = Sales.Currency.CurrencyCode
    join Person.CountryRegion on Sales.SalesTerritory.CountryRegionCode = Person.CountryRegion.CountryRegionCode
    order by AccountNumber
```

Она то и используется в коде C# по причине своей большей безопасности и производительности.

БД **ApplicationInsights** - туда в таблицу **Insights** записываются исключения и успешные действия программы

При отсутствии исключений:

![Screenshot](Screenshots/2.png)

При исключении:

![Screenshot](Screenshots/3.png)

Хранимые процедуры:

    Хранимая процедура для очистки таблицы:

```sql
USE [ApplicationInsights]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_iDENTIFIER ON
GO
ALTER procedure [dbo].[sp_GetCustomers]
as
    delete Insights
```

    Хранимая процедура для получения данных, в будующем используемых в коде для записи
```sql
USE [ApplicationInsights]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_iDENTIFIER ON
GO
ALTER procedure [dbo].[sp_GetCustomers]
as
    select Message, Time from Insights
```

 *Сlass library* сформирован для удобства, а всё содержимое находится в папке **ServiceLibrary** с *ConfigManager*, `xml`- и  `json`-парсерами. Проект подключён как зависимость к обеим службам.

Вся функциональность **LINQ to XML** содержится в пространстве имен *System.Xml.Linq*, а основная функциональность по работе с **JSON** сосредоточена в пространстве имен *System.Text.Json*. Представление в памяти **схемы XML** *System.Xml.Schema*.

Объект настроек для нашего **DataManager** - класс **DataOptions** (пример xml и json файлов настроек в папке **dataOptions**)

```c#
public class DataOptions
{
    public string ConnectionString { get; set; }

    public string LoggerConnectionString { get; set; }

    public string SourcePath { get; set; }

    public string OutputFolder { get; set; }

    public DataOptions() { }
}
```

* **ConnectionString** - строка для подключения к БД.

* **LoggerConnectionString** - строка для подключения к базе данных Logger.

* **SourcePath** - путь к File Transfer Protocol.

* **OutputFolder** - папка, где изначально будут создаваться XML файлы с данными БД и XML файлы из БД Logger. Оттуда и будут пересылаться в *SourcePath*  файлы.

Классы для работы с БД находятся в папке **ServiceLibrary**.

Класс **DataIO** предназначен для работы с данными в БД *ApplicationInsights* и чтения данных из *AdventureWorks2019*. В качестве *sql* команд используются хранимые процедуры (приведены выше)

Класс **FileTransfer** принимает в конструктор *OutputFolder* и *SourcePath* и перемещает *fileName* на FTP.

Класс **XML_Generator** генерирует XML файл в *OutputFolder*. Рядом с XML файлом генерируется XSD файл, валидирующий его.

В БД *ApplicationInsights* ведётся регистрация всех действий. По окончании работы службы из нее  формируется XML и XSD файл в *OutputFolder* для чтения данных.

The end. :raised_hands:
