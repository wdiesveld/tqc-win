# tqc-win

Windows commandline interface for the TinyQueries Compiler API.

## Installation

- Copy `./bin/tqc.ps1` to a folder which is in your PATH
- Create a file `tinyqueries.env` which contains your TinyQueries&trade; API key:
  ```
  echo "TQ_API_KEY=<YOUR-API-KEY>" > tinyqueries.env
  ```
  _Note: If you have more than one API-keys you can put a `tinyqueries.env` file in each of your project folders, instead of having one globally defined API key._

## Compiler Configuration

Make sure you have a compiler configuration file for each of the projects for which you need to compile queries. The config settings for the compiler are stored in the `tinyqueries.json` or `tinyqueries.yml` file. For a details see https://compile.tinyqueries.com

## Usage

- CD to the folder which contains a `tinyqueries.json` or `tinyqueries.yml` file
- Run `tqc`
- The compiled output files will be put in your project folder as configured in your config file.

## References

API Documentation: https://compile.tinyqueries.com
General Info: https://www.tinyqueries.com

