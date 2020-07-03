## File Destination Adapter

Use the File destination adapter to write messages to files or to return messages read from files depending on which operation it's been configured for.

There are 4 operation modes to choose from.

### 'Write' mode:
The File destination adapter gets the messages from the Message Engine and 
writes files to local file system or to network shares.

```
Note:
    The adapter will try to create missing directories. 
```
```
Tip:
    If setting 'Path' property to something like C:\Something\%date% the adapter will 
    expand the %date% variable into the current date.
```


### 'Read' and 'ReadAndDelete' mode:
When any of these modes are selected the adapter will try reading files specified byt the 
configuration properties Path and Filename.
If Filename property contains wildcard characters multiple files can be returned.

The only difference between the two modes is that Read will never try to delete the read file,
whereas ReadAndDelete will try to delete the file after it's been read.

```
Tip:
    If setting 'Filename' property to %CompleteMessage% the adapter will 
    read files depending on incoming message content. 
```

### 'None' mode:
As the name implies, the adapter will do nothing when this is selected.


### Rx
```
Rx Note:
    When using the adapter in Rx mode the inbound message will be read directly 
    from the Rx-Stream.
```