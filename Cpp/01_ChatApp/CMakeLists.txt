# Copyright 2018 gRPC authors.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#
# cmake build file for C++ helloworld example.
# Assumes protobuf and gRPC have been installed using cmake.
# See cmake_externalproject/CMakeLists.txt for all-in-one cmake build
# that automatically builds all the dependencies before building helloworld.

cmake_minimum_required(VERSION 3.23.3)

project(ChatApp C CXX)

include(../cmake/common.cmake)

# Specify the C++ standard
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED True)

# Proto file
get_filename_component(proto "./Shared/chat.proto" ABSOLUTE)
get_filename_component(proto_path "${proto}" PATH)

# Generated sources
set(proto_srcs "${CMAKE_CURRENT_BINARY_DIR}/chat.pb.cc")
set(proto_hdrs "${CMAKE_CURRENT_BINARY_DIR}/chat.pb.h")
set(grpc_srcs "${CMAKE_CURRENT_BINARY_DIR}/chat.grpc.pb.cc")
set(grpc_hdrs "${CMAKE_CURRENT_BINARY_DIR}/chat.grpc.pb.h")
add_custom_command(
      OUTPUT "${proto_srcs}" "${proto_hdrs}" "${grpc_srcs}" "${grpc_hdrs}"
      COMMAND ${_PROTOBUF_PROTOC}
      ARGS --grpc_out "${CMAKE_CURRENT_BINARY_DIR}"
        --cpp_out "${CMAKE_CURRENT_BINARY_DIR}"
        -I "${proto_path}"
        --plugin=protoc-gen-grpc="${_GRPC_CPP_PLUGIN_EXECUTABLE}"
        "${proto}"
      DEPENDS "${proto}")

# Include generated *.pb.h files
include_directories("${CMAKE_CURRENT_BINARY_DIR}")

# grpc_proto
add_library(grpc_proto
  ${grpc_srcs}
  ${grpc_hdrs}
  ${proto_srcs}
  ${proto_hdrs})
target_link_libraries(grpc_proto
  ${_REFLECTION}
  ${_GRPC_GRPCPP}
  ${_PROTOBUF_LIBPROTOBUF})

# # client
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