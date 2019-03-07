### Change log:

-- **18-02-2019** *covered only the major changes*

- Database
  - migrations will executed on run-time. This mean `add-migration` and `update-database` can be omitted.
  - helpers class to run stored procedure and indexes `App_Data\Install\*.sql`

- Install module
  - replace seeder on existing application.
  - covered-up waiting time on application first-load startup.
  - helps prepared database and overcome previous issues (database not loaded on startup)
  - introduced sysadmin configuration
    ```
    With some of modules need to configure first such as API Settings,
    (others still hard-coded), sysadmin will be the highest in role`s
    hierarchy and have controls over it.
    ```

- Identity framework
   - omitted, to cater Restful API
   - `UserStore`,`RoleStore` and `SignInManager` replaced with function that acted just like its current implementation (use ``UserService`` instead)

- API
   - approach: Restful plugin-based 
   - [documentation](https://documenter.getpostman.com/view/4900831/RztrHRUB)

-- **21-02-2019**

- added DownloadMasterData to background worker and scheduled tasks.

-- **07-03-2019**

- enhanced panel design with taghelper
- Store
   - common CRUD operations
   - assign user 
   - mapping
- Device
   - change to inline editor