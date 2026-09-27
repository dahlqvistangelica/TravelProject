![Schema](schema-sql-travel-png)

För att köra: 
 1. Kör database-rebuild-all.ps1 i _scripts-mappen.
 2. Kör därefter initDatabase.sql i DbConfig/SqlScripts
 3. Därefter går det att bygga och köra. 

## Tankar kring klasserna
Jag har valt att ha både categories, citys och countries i egna tabeller eftersom det underlättar skalbarhet i framtiden. Det är mer komplicerat att bygga upp än t.ex. en enum, men vid en större applikation kommer det att underlätta att kunna lägga in egna städer och länder i efterhand. Jag har dock valt att inte ta med controllers för dessa eftersom det inte efterfrågades i uppgiften. Ändring av stad och land går att göras ändå genom att t.ex. byta adress på attraktionen. 
Den här gången valde jag också att varje attraktion enbart kan ha en kategori också, men varje kategori kan ha många attraktioner. 
Jag har också valt att varje adress enbart kan ha en attraktion, det är ju en smaksak, men i det här fallet tänkte jag att majoriteten av olika attraktioner faktiskt har en egen adress och inte heller delar med någon, därför valde jag att göra så. Jag har också lagt in så man kommer åt både attraktionen från adressen men också adressen från attraktionen, för att det ska vara sökbart från båda hållen i endpointsen. 
Det enda index jag skapat denna gång är faktiskt för adresserna, för att de ska vara unika och inte ha en massa dubbletter. 
