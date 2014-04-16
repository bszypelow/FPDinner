FPDinner
========

Aplikacja demonstracyjna do wykładu RavenDB - nie tylko SQL, której celem jest przedstawienie podstawowych koncepcji i sposobów użycia dokumentowej bazy danych.

Uwaga: Tego demo nie należy traktować jako gotowej aplikacji nadającej się do pracy w środowisku produkcyjnym. W szczeglności:

1. Zupełnie pominięto walidację danych wejściowych i obsługę wyjątków.
2. Brakuje reguł autoryzacyjnych.
3. Nie ma żadnych testów automatycznych.

******************

In order to use the Intranet template, you'll need to enable Windows authentication
and disable Anonymous authentication.

IIS 7 & IIS 8
-------------

1. Open IIS Manager and navigate to your website.
2. In Features View, double-click Authentication.
3. On the Authentication page, select Windows authentication. If Windows authentication is not an option, you'll need to make sure Windows authentication is installed on the server.

   To enable Windows authentication on Windows:
      
   1. In Control Panel open "Programs and Features".
   2. Select "Turn Windows features on or off".
   3. Navigate to Internet Information Services > World Wide Web Services > Security and make sure the Windows authentication node is checked.


   To enable Windows authentication on Windows Server:

   1. In Server Manager, select Web Server (IIS) and click Add Role Services
   2. Navigate to Web Server > Security and make sure the Windows authentication node is checked.

4. In the Actions pane, click Enable to use Windows authentication.
5. On the Authentication page, select Anonymous authentication.
6. In the Actions pane, click Disable to disable anonymous authentication.

IIS Express
-----------
 
 1. Click on your project in the Solution Explorer to select the project.
 2. If the Properties pane is not open, open it (F4).
 3. In the Properties pane for your project:

    1. Set "Anonymous Authentication" to "Disabled".
    2. Set "Windows Authentication" to "Enabled".
