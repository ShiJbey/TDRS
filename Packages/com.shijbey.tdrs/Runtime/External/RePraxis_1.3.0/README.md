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
db.Insert( "astrid.relationships.jordan.tags.friend" );
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

if ( result.Success )
{
    // Print the results
    Console.WriteLine(result.ToPrettyString());

    foreach (Dictionary<string, object> binding in result.Bindings)
    {
        // Do something
    }
}


// Optionally you could also pass initial bindings to a query to limit the results

result = new DBQuery()
    .Where( "astrid.relationships.?other.reputation!?r" )
    .Where( "gte ?r 10" )
    .Run( db, new Dictionary<string, object>() { { "?other", "lee" } } );
```

More examples are available under the `tests` directory.

### Query statement types

Statements in the query are ran sequentially in the same order they were provided. Some statements have slightly different semantics based depending if they contain variables or are the first statement in the query.

#### Assertion statement

This statement when used without variables will check if the statement holds true within the database. If the statement does not hold true, the query will stop evaluating and return a failed result.

```csharp
result = new DBQuery()
    .Where( "astrid.relationships.jordan.reputation!30" )
```

If variables are provided, the statement will find all entries in the database that can bind to those variables. All bindings are saved to the query result. If no valid bindings are found, the query ends and returns a failed result. If an assertion with variables has preceding statements or initial bindings were provided to the query, the statement will filter the existing bindings for those that satisfy it.

```csharp
// Try to find all ?other, where astrid's reputation to that ?other is 30
result = new DBQuery()
    .Where( "astrid.relationships.?other.reputation!30" )
```

```csharp
// You can also use multiple variables in the same statement
// Below we bind the target of all astrid's relationships and their corresponding
// reputation value, to the variables ?other and ?rep.
result = new DBQuery()
    .Where( "astrid.relationships.?other.reputation!?rep" )
```

```csharp
// Below we get all astrid's relationships with the 'friend' tag (jordan and lee)
// Next we filter those bindings for those where the reputation is 20
// This will limit the result to ?other=lee
result = new DBQuery()
    .Where( "astrid.relationships.?other.tags.friend" )
    .Where( "astrid.relationships.?other.reputation!20" )
```

#### Not-statement (negation)

Not-statements are slightly tricky to understand. Their meaning changes based on the existence of variables, existing bindings, and their position in the statement order. Also while you may use variables within a not statement, that statement can never bind new values to variables, it is purely for filtering.

If a not-statement appears as the first statement in a query, and the statement does not have any variables, the statement succeeds if the sentence being negated does not appear in the database. This case is not affected by the statement's position in the order. If the statement fails, the entire query fails.

```csharp
// The following succeeds because "astrid.relationships.jordan.reputation!20" is
// not a true statement in the database
var result = new DBQuery()
    .Where( "not astrid.relationships.jordan.reputation!20" )
    .Run( db );
```

```csharp
// The following fails because "astrid.relationships.jordan.reputation!30" is
// a true statement in the database
var result = new DBQuery()
    .Where( "not astrid.relationships.jordan.reputation!30" )
    .Run( db );
```

Not-statements that contain variables change meaning based on existing bindings/execution order. If a not-statement appears first in the query and has variables, then the statement passes if there does not exist an entry in the database that makes the sentence in the statement true. See the example below.

```csharp
// This query succeeds when there does not exist an ?other that astrid has a relationship with
var result = new DBQuery()
    .Where( "not astrid.relationships.?other" )
    .Run( db );
```

Consider the query below. When working with values, you might be tempted to think that the query below binds all `?other` where the reputation is not 15. That is **incorrect**. Like the query above this statement passes if there is no `?other` for which astrid's reputation toward them is 15.

```csharp
// Passes when: for all relationships astrid has with all ?others,
// no relationship has a reputation of 15
var result = new DBQuery()
    .Where( "not astrid.relationships.?other.reputation!15" )
    .Run( db );
```

If you wanted to get all relationships to other where reputation does not equal 15, you need to use two separate statements. The code below first binds all "others" and their corresponding reputation values, then it filters for those not equal to 15. We discuss relational statements in the next section.

```csharp
var result = new DBQuery()
    .Where( "astrid.relationships.?other.reputation!?rep" )
    .Where( "neq ?rep 15" )
    .Run( db );
```

If the not-statement is preceded by other queries or initial bindings are provided, then the statement filters all intermediate query bindings for those where the statement does not hold.

```csharp
// Given that ?other is britt, the statement "astrid.relationships.?other.reputation!30"
// is not true. So, the query passes.
var result = new DBQuery()
    .Where( "not astrid.relationships.?other.reputation!30" )
    .Run( db, new Dictionary<string, object>()
    {
        {"?other", "britt"}
    } );
```

```csharp
// First the query binds all ?others that astrid has a relationship with,
// then the not-statement filters those results for those where the statement
// is not true. This query would return britt and lee as valid bindings of other
var result = new DBQuery()
    .Where( "astrid.relationships.?other" )
    .Where( "not astrid.relationships.?other.reputation!30" )
    .Run( db );
```

#### Relational statements

Relational statements are used to check for equality/inequality. Each statement starts with an operation name followed by two values. These values must be be variables, single symbols (strings), or integers/floats. You **cannot** pass a sentence as a parameter. You should first bind your value of interest to a variable, then use it in a relational statement.

- `eq a b`: Checks if `a` is equal to `b`
- `neq a b`: Checks if `a` is not equal to `b`
- `gt a b`: Checks if `a` is greater than `b`
- `lt a b`: Checks if `a` is less than to `b`
- `gte a b`: Checks if `a` is greater than or equal to `b`
- `lte a b`: Checks if `a` is less than or equal to `b`

Below is an example query provided earlier that uses the `neq` (not equal) operator to filter results.

```csharp
var result = new DBQuery()
    .Where( "astrid.relationships.?other.reputation!?rep" )
    .Where( "neq ?rep 15" )
    .Run( db );
```

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
