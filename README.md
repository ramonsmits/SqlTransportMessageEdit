# SqlTransportMessageEdit
Tool to export/import message from NServiceBus SqlTransport queue table for editing


## Usage

1. Modify `App.config` so that it contains the required connection strings.
2. Run the console application and specify the connection string key to use, the table to read/write from/to and optionally the message ID (guid).


### Arguments

`sqlmessageedit.exe {connectionstring key} {queue table name} ({message id})`

Example:

`sqlmessageedit.exe local dbo.error`

This will result in a list of message currently in the table:
```
   0: 27d5b034-2edf-4e64-826d-a56900b63d88
   1: 313a0e18-1b30-4cd7-b061-a56900b63c7f
   2: 3ea962d3-c753-4191-847c-a56900b63c27
   3: 5af7d8cd-a54a-443d-9a27-a56900b63d31
   4: 62259d8e-c7b9-4783-8a4b-a56900b63dd7
   5: 690575e1-cf71-49bf-a89c-a56900b63c52
   6: 7aeb6b44-ca67-495b-be9b-a56900b63d5a
   7: 81f99173-1bf4-484d-9103-a56900b63d03
   8: 862026fb-b9bb-4af2-b63e-a56900b63bfe
   9: a1bdb559-29fc-4b0a-a045-a56900b63cab
  10: cbe1a687-5384-4414-bed7-a56900b63e04
  11: d0d0ad20-59a0-4d17-8b8f-a56900b63dae
  12: dec41279-b960-465e-9c67-a56900b63cd8
```

You can then enter either the row number or the message uuid. After that you must confirm the editing of the message body.

The message body will now be read and exported to a temporary file and the filename will be written to the console.

The application will wait until editing of the file completes.

Updating of the message body needs to be confirmed by entering `UPDATE` which will import the modified file and update the table row.

## Encoding

The exported file contains a binary dump of the body column. The import will do a binary read too to prevent text encoding issues.

## Supported message bodies

The tool tries to guess the body content by inspecting the first few characters and set the file extension to `json`, `xml` or `bin`.
