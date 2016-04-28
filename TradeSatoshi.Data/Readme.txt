To add new migration after change in domain model:
>add-migration <descriptive-name>

To update database - upgrade:
>update-database 

To update database - downgrade:
>update-database -TargetMigration:<migration-name>


For more info refere to EF docs: https://msdn.microsoft.com/en-nz/data/jj591621.aspx