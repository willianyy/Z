IMPORTANT NOTES:

## LightSpeed ORM Binaries
The library does not ship with LightSpeed ORM binaries, and it expects the binaries to be available at the default installation path: `C:\Program Files (x86)\Mindscape\Bin\`.

## Identity Generation Method
Currently only `IdentityMethod.IdentityColumn` is supported for identity generation method in order to preserve full compatibility with OrmLite implementation.

## UserAuth Roles
Role addition and removal is only supported through the repository class. Running collection methods over the UserAuth's `Roles` and `Permissions` properties (e.g. `userAuth.Roles.Add(newRole)`) will not persist the changes.