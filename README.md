# DressingFramework - DK
A framework that assembles DressingTools and allows the creation of modular plugins for avatars.

> :warning: Very unstable API. Currently, it is only intended to use in my own tools internally. Use at your own risk. :warning:

## Features

- Automatic non-destructive build while uploading avatars or entering play mode
- Sequencing passes executions by specifying before/after constraints and categorizing into different stages
- Animation scanning and path remapping are supported
- Animator and menu cloning and manipulation in framework-level
- Non-VRChat workflows supported (Experimental)
- Acts as an avatar and build abstraction layer on top of other non-destructive frameworks
  - Supports NDMF currently and allows to run the same pass in multiple runtimes (Experimental)

### WIP

- Standardized logging and reporting system
- Platform-independent avatar dynamics scanning
- Reading values from DynamicBones and VRCPhysBone without them installed (WIP)

## License

This project is licensed under the LGPLv3 License. [tl;dr](https://tldrlegal.com/license/gnu-lesser-general-public-license-v3-(lgpl-3))
