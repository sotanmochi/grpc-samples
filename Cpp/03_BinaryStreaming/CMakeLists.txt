cmake_minimum_required(VERSION 3.23.3)

project(BinaryStreaming C CXX)

include(../cmake/common.cmake)

# Specify the C++ standard
set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED True)

# grpc sources
set(grpc_srcs "./Shared/binarystreaming.grpc.pb.cc")
set(grpc_hdrs "./Shared/binarystreaming.grpc.pb.h")
include_directories("./Shared")

# grpc_proto
add_library(grpc_proto
  ${grpc_srcs}
  ${grpc_hdrs})
target_link_libraries(grpc_proto
  ${_REFLECTION}
  ${_GRPC_GRPCPP}
  ${_PROTOBUF_LIBPROTOBUF})

# client
add_executable(client "./Client/main.cpp")
target_link_libraries(client
  grpc_proto
  ${_REFLECTION}
  ${_GRPC_GRPCPP}
  ${_PROTOBUF_LIBPROTOBUF})

# server
# add_executable(server "./Server/main.cpp")
# target_link_libraries(server
#   grpc_proto
#   ${_REFLECTION}
#   ${_GRPC_GRPCPP}
#   ${_PROTOBUF_LIBPROTOBUF})