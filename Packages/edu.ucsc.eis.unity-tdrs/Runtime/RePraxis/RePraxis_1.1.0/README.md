# Re:Praxis - In-memory database and query language for C\#

Re:Praxis is an in-memory database solution for creating simple databases for games and applications. It is a reconstruction of Praxis, the exclusion logic-based language used by the [Versu social simulation engine](https://versu.com/). Users store information using strings called *sentences*, and the system parses these to create an internal database tree. Users can then query for patterns in the data using the same syntax used to store information.

## Installation

1. Find and Download the latest release of Re:Praxis under the [Releases](https://github.com/ShiJbey/RePraxis/releases) page. Select the `RePraxis_X.Y.Z.zip` entry under "Assets" (where `X.Y.Z` corresponds to the Re:Praxis version).
2. Unzip the download to produce a `RePraxis` directory. This directory should contain the `RePraxis.dll`, `RePraxis.pdb`, and `RePraxis.deps.json` files generated from building the source code.
3. Copy this directory into your project.
   - If you're using Unity, place this directory within a directory named `Plugins` within your `Assets` folder.

## Creating a new database

Creating a new database is the first thing you need to do. A `RePraxisDatabase` instance is responsible for managing all the data and providing data to query.

```csharp
using RePraxis;

// Construct a new database
RePraxisDatabase db = new RePraxisDatabase();
```

## Adding information

Adding information to the database is easy. There are no SQL-like tables or property-driven documents. Re:Praxis stores information using strings of text called sentences. Sentences are broken into nodes using the dot (`.`) and exclusion (`!`) operators (See the example code below).

You can think of the internal structure of the database as a tree. Nodes followed by the dot operator can have more than one child, and those followed by the exclusion operator can only have a single child (See the example below).

```csharp
// The sentence below enters the fact that we have a symbol "ashley" that has
// a child symbol "age", and "age" can have only one child value, which is currently
// set to 32
db.Insert( "ashley.age!32" );

// The sentence below enters another fact into the database. It has a symbol ashley
// with a property likes, with a child "mike", which indicates that ashley likes
// mike
db.Insert( "ashley.likes.mike" );
```

This setup allows us to express and store various things like character stats and relationships. For more information about the syntax, please see [this presentation](https://versublog.files.wordpress.com/2014/05/praxis.pdf) by Richard Evans on the original Praxis language.

## Deleting information

Conversely, we can remove information from the database using the same syntax. The code below removes this entry from the database and any other data entries prefixed with `"ashley.likes.mike"`.

```csharp
db.Delete( "ashley.likes.mike" );
```

## Asserting information

Users can check if the database has a piece of data using the `RePraxisDatabase.Assert` method. `Assert` will return false if the data is not in the database, the values differ, or the cardinalities (`.` or `!`) don't match.

```csharp
db.Assert( "ashley.dislikes.mike" );
// Returns false

db.Assert( "ashley.likes.mike" );
// Returns true

db.Assert( "ashley.likes" );
// Returns true since there are entries with this as a prefix
```

## Querying the database

Finally, the most powerful part of this database solution is the ability to query for patterns using variables. Queries have an extended syntax allowing variables, negations, and relational operations. The example below creates a database and fills it with information about relationships between some characters and a player. Then we create a new query that looks for valid bindings for the variables: `?speaker`, `?other`, `?r0`, and `?r1`.

Note that variables are specified using a question mark (`?`). The question mark **does not** replace the `.` or `!`.

```csharp
using RePraxis;

RePraxisDatabase db = new RePraxisDatabase();

// Add new information into the database
db.Insert( "astrid.relationships.jordan.reputation!30" );
db.Insert( "astrid.relationships.jordan.tags.rivalry" );
db.Insert( "astrid.relationships.britt.reputation!-10" );
db.Insert( "astrid.relationships.britt.tags.ex_lover" );
db.Insert( "astrid.relationships.lee.reputation!20" );
db.Insert( "astrid.relationships.lee.tags.friend" );
db.Insert( "player.relationships.jordan.reputation!-20" );
db.Insert( "player.relationships.jordan.tags.enemy" );

// Query for a pattern and get valid bindings for variables
QueryResult result =
    new DBQuery()
        .Where( "?speaker.relationships.?other.reputation!?r0" )
        .Where( "gt ?r0 10" )
        .Where( "player.relationships.?other.reputation!?r1" )
        .Where( "lt ?r1 0" )
        .Where( "neq ?speaker player" )
        .Run( db );

if ( results.Success )
{
    // Print the results
    foreach ( var bindingDict in results.Bindings )
    {
        Console.WriteLine(
            "{"
            + string.Join(
               ", ", bindingDict.Select( b => b.Key + "=" + b.Value ).ToArray()
            )
            + "}"
        );
    }
}

// Output:
//  {?speaker=astrid, ?other=jordan, ?r0=30, ?r1=-20}

// Optionally you could also pass initial bindings to a query to limit the results

result = new DBQuery()
    .Where( "astrid.relationships.?other.reputation!?r" )
    .Where( "gte ?r 10" )
    .Run( db, new Dictionary<string, string>() { { "?other", "lee" } } );
```

More examples are available under the `tests` directory.

### Query Operators

- `not <sentence>`: Checks if a sentence is not present in the database
- `eq a b`: Checks if `a` is equal to `b`, where `a` and `b` must be variables, single symbols, or integers/floats.
- `neq a b`: Checks if `a` is not equal to `b`, where `a` and `b` must be variables, single symbols, or integers/floats.
- `gt a b`: Checks if `a` is greater than `b`, where `a` and `b` must be variables, single symbols, or integers/floats.
- `lt a b`: Checks if `a` is less than to `b`, where `a` and `b` must be variables, single symbols, or integers/floats.
- `gte a b`: Checks if `a` is greater than or equal to `b`, where `a` and `b` must be variables, single symbols, or integers/floats.
- `lte a b`: Checks if `a` is less than or equal to `b`, where `a` and `b` must be variables, single symbols, or integers/floats.

## Building Re:Praxis from source

Building Re:Praxis from source requires that you have .Net net installed. Run the following commands, and new `RePraxis.dll` and `RePraxis.pdb` files will be generated within the `dist` directory.

```bash
# Step 1: Clone the repository
git clone https://github.com/ShiJBey/RePraxis.git

# Step 2: Change to the project repository
cd RePraxis

# Step 3: Build using dotnet CLI
dotnet build
```

## References

- <https://versu.com/about/how-versu-works/>
- <https://github.com/JamesDameris/Wyclef>
- <https://github.com/mkremins/praxish>
