## Windows Storage

> https://mp.weixin.qq.com/s/QNNnrLS2z6_CWGwRwqlSkg

以下是 Windows 系统下，几个不同安装软件目录之间的区别:

- `C:\Program Files`: 这个目录通常用于安装 64 位应用程序。只有管理员才能在这个目录中安装软件，但所有用户都能使用该软件。若程序数据也写在这个目录下，软件每次启动都会要求授予管理员权限。
- `C:\Program Files (x86)`: 这个目录通常用于安装 32 位的应用程序。只有管理员才能在这个目录中安装软件。

此外，目前很多软件安装在 `C:\Users\UserName\AppData` 目录中，并造成一定程度的滥用:

- `C:\Users\UserName\AppData\Roaming`: 可在资源管理器的地址栏输入 `%AppData%` 访问该目录。这里用来存放当前登录用户所产生的数据，对其他的用户不可见。同时该目录是可以与服务器同步。如果在公司网络加入了域，这个文件夹会通过网络同步，让数据可随用户从一台电脑移动到另一台电脑。
- `C:\Users\UserName\AppData\Local`: 可在资源管理器的地址栏输入 `%LocalAppData%` 访问该目录。这个目录和 Roaming 目录恰好相反，数据仅限本地，加入域也不会同步。除此以外其他方面并没有什么区别。
- `C:\Users\UserName\AppData\Local\Programs`: 用户个人使用的不加入域的软件可安装在该目录。
- `C:\Users\UserName\AppData\LocalLow`: 该目录包含的数据也不能移动，且具有更低的访问级别。如果在保护模式或安全模式下运行一个网络浏览器，应用程序可能只能访问 LocalLow 文件夹中的数据。

此外还有一些其他的目录:

- `C:\ProgramData`: 与 AppData 中文件区别在于，该目录包含的是全局应用数据，而非特定于用户。这些数据对计算机上的所有用户都是可用的。
- `C:\Users\UserName`: 相当于 Linux 的 home 目录。