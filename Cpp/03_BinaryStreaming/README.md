# Chat app project - gRPC code samples for C++

## Install gRPC
https://grpc.io/docs/languages/cpp/quickstart/#install-grpc

## Build
```
$ export MY_INSTALL_DIR=$HOME/.local
$ printenv MY_INSTALL_DIR
```
```
$ mkdir -p build
$ pushd build
$ cmake -DCMAKE_PREFIX_PATH=$MY_INSTALL_DIR ..
$ make -j
```
