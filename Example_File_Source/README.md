## File Source Adapter
Use the File source adapter to receive messages from files.
The adapter reads the file, and creates a message object, so that it can be processed.

```
Note:
    The adapter does not pick up any read-only, temporary or system files. 
```

The File source adapter reads the messages from files on local file systems or on network shares.
After the message has been read it will be pushed to the Messaging Engine for further processing.
When message processing is complete the file will be deleted by the adapter from the file system or network share.

```
Rx Note:
    When using the adapter in Rx mode the inbound file will be deleted directly 
    after it's been read and sent to the Rx-Stream.
```