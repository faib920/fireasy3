create tablespace northwind
datafile 'D:\oracle\oradata\ORCL\northwind.dbf'
size 50m
autoextend on
next 50m maxsize 20480m
extent management local;

create user c##test identified by Faib1234 default tablespace northwind;

grant connect,resource,dba to c##test;
grant unlimited tablespace to c##test;
