# NorthwindCache
Example of Anonymous Pipe. Use Anonymous Pipe to communicate with client and cache process

1. Run the clinet.exe. it will start server.exe (it should name cache.exe)

2. server.exe will create pipe and load whole Northwind's Categories table to memory

3. client get input id and sent it to cache server.

4. server get the record row and serialize it (if used JSON.NET will get better performence)

5. clinet get the serialized data and deserialize it

6. show the result to console

----

Why need this?

Because I have a scenario on my work that is:

a legacy program do ten thousand time queries to generate a report. it take a very long time.

(db is MSSQL 2000, it is another legacy and suck performence)

and it is legacy code so no one know the code logic.

I have to reduce the executive time, so I decide to load whole table and do in-memory query.

another problem is the server which run the program is 32 bit win2003.

it can not hold 3 tables in a process, because each table has million row and it cause out of memory.

So I make the multi-process cache like this.

Finally, the running time from 2.5 hour reduce to 5 min.
