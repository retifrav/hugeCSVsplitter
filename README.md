# hugeCSVsplitter

A GUI application for splitting huge CSV files into several smaller ones. Since it is a [WPF](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)-based application, it only supports Windows platforms (*but perhaps one could also try to run it through Wine/Proton on other platforms*).

![hugeCSVsplitter main window screenshot](/img/mainwindow.png?raw=true "hugeCSVsplitter main window screenshot")

There are some settings in the `.config` file (*should be deployed alongside the executable*). More details in the application help window (*press F1 to open it*).

And some more details in [this article](https://decovar.dev/blog/2014/12/16/hugecsvsplitter/) (*in russian*).

## Requirements

- .NET Framework 4.8 or later (*depends on the particular application version/build*)

## 3rd party

- [Ookii.Dialogs](https://ookii.org/Software/Dialogs/) for the file/folder dialogs
    + `Ookii.Dialogs.Wpf.dll` needs to be deployed alongside the executable
- icons from [Iconfinder](https://iconfinder.com/) (*might be violating the service terms*)
