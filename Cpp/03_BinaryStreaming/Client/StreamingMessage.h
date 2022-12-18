#include <cstdint>
#include "msgpack.hpp"

namespace BinaryStreamingApp::Client
{
    class StreamingMessage
    {
        public:
            // Default constructor for deserialization
            StreamingMessage(){}

            StreamingMessage(uint64_t timestamp, std::string textmessage)
            {
                _timestamp = timestamp;
                _textmessage = textmessage;
            }

            // Serialization Settings. Specify the member variable to be serialized.
            MSGPACK_DEFINE(_timestamp, _textmessage);

            const uint64_t timestamp() { return _timestamp; }
            const std::string& textmessage() { return _textmessage; }

            void set_timestamp(uint64_t value) { _timestamp = value; }
            void set_textmessage(const std::string& value) { _textmessage = value; }

        private:
            uint64_t _timestamp;
            std::string _textmessage;
    };
}