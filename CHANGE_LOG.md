### Change log:

##### **18-02-2019** *covered only the major changes*

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

- API v1.0.1
   - approach: Restful plugin-based 
   - [documentation](https://documenter.getpostman.com/view/4900831/RztrHRUB)

##### **21-02-2019**

- added DownloadMasterData to background worker and scheduled tasks.

##### **07-03-2019**

- enhanced panel design with taghelper
- Store
   - common CRUD operations
   - assign user 
   - mapping
- Device
   - change to inline editor

##### **19-03-2019**

- API v1.0.1
   - added webhook for HTTP callback/notification (refers to [API documentation](https://documenter.getpostman.com/view/4900831/RztrHRUB) for the usage explanation)
   - updated DTOs object
- implemented events triggering (see IConsumer & IEventPublisher)
- added picture services
- enhanced **Settings** module (currently applied to security, general, users and media)

##### **11-07-2019** [branch: dev_v1.2.2](http://172.20.2.63:5000/esd/myNEWS-StockManagement-Web/commits/dev_v1.2.2)

- API v1.2.0
	- factorized most of the services to be more abstract.
	- standardized query parameters usage to snake case (will take Json property name on Dto`s classes)
		- previous: `/search?query=p_branchno:123&fields=branch_no`
		- current: `/search?query=branch_no:123&fields=branch_no`
	- added pre-validation on few attributes. 
	- a little house keeping for the performance-wise

##### **4-9-2019**

- License Manager
   - used .NET Core ported [Portable.Licensing](https://github.com/CoreCompat/Portable.Licensing) to create key pair for device serial no
   - add common CRUD manager
- API v1.2.2
   - update associated API-related to handle license creation
   - update middle-ware pipeline 