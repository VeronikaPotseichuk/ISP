# Потейчук Вероника 953502

## Описание
Лабораторная №3 представляет собой усовершенствованный вариант Второй лабораторной. То бишь Windows-службу, осуществляющую мониторинг, архивацию и деархивацию файлов и папок, перемещение, изменение состояния файла и др.

### Что же изменилось?
Теперь можно задавать направление (путь) нашей службе, а не заставлять её работать по заранее написанному. Следовательно, она может перемещаться по директориям. А вот чтобы это делать, и требуются __config-файлы__ (в них и лежат требования).

### Программа содержит такие файлы, как:

* ***_Parser_***
  
  __JSON_Parser__ - это парсер для файлов `.json`. Содержит описание интерфейса, который в себе структурирует файл в объект типа `JSON_Options`, а метод возвращает объект типа `Options`.
  
  __XML_Parser__ - парсер для файлов `.xml`. Аналогичен _JSON_Parser_, однако пред тем, как доставать проперти из файла, необходимо для начала убедиться в его допустимости (то бишь валидности).
  
  __IParser__ - а вот и наконец сам интерфейс, содержащий метод `_ParseOptions()_` типа `Options`.

* ***_Options_***

  __JSON_Options__ - это класс, служащий для парсинга файлов с типом `.json`. В ходе структуризации все свойства будут лежать в объекте сего класса.
  
  __XML_Options__ - этот класс аналогичен вышеописанному, однако предназначен для файлов `.xml`.
  
  __Options__ - это класс является конечным, т.к. в его объекте будут лежать свойства нашего файла. Инициализация происходит в парсерах. Всё зависит от типа используемого файла. __ConfigManag__ вызывает парсер какого-то типа, а метод _ParseOptions_ возвращает объект этого класса.
  
* ***_Service1_*** - это и есть файл нашей Windows-службы, содержайщий усовершенствованный (в сравнении с предыдущей лабораторной) класс `Logger`. Теперь он может работать с необходимыми типами файлов. 
  
* ***_ConfigManag_*** - вернёмся чуть выше к классу `Options`, где и как используется сей класс.
