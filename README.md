About
===============
    
Say, you have a really huge CSV file, like 1 GB or even more. What to do with it? No common application will open it normally. Think - you need 1 GB free RAM to do so. And what if it's 5 GB? Or 10?

The solution is: split that file into several smaller files, which can be delt with. This applicatin is dedicated to do such thing.

![hugeCSVsplitter main window screenshot](/img/mainwindow.png?raw=true "hugeCSVsplitter main window screenshot")

All you need to do is grab your file and drop it in the dashed drop area. Or you can specify the path to the file in the first text filed. The next thing - you specify the output directory, where splitted files will be saved. You can leave that field blank and files will be saved to the source CSV file directory.

Settings
===============

You can set some splitting settings in the `.config` file:

```linesPerFile

How mush lines will trigger the splitting into another file. I'd recommend value `500k` or more if strings in you CSV are rather short and `100k` or less if they are pretty long. Anyway, don't set very big values for the obvious reasons.

```addHeader

Set `true` if you want to add the first string (header) of the source file to all splitted parts, set `false` if you don't. Default value is `true`.

3rd party
===============
The application is written in C#, WPF with Visual Studio 2013 (if that matters) and .NET 4.5.1. Those nasty bastards did all the work, I just wrote a few lines of code.

Also I used [Ookii.Dialogs](http://www.ookii.org/Software/Dialogs/) for open file/folder dialogs, because for some reasons there is no such thing as OpenDirectoryDialog in WPF.

Also I snatched some (all) icons from [Iconfinder](https://www.iconfinder.com/).