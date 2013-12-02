using System;
using System.Collections.Generic;

namespace DropNet.Models
{
    public class ChunkedUploadProgress
    {
        private readonly string _uploadId;
        private readonly long _chunksCompleted;
        private readonly long _bytesSaved;
        private readonly long _retryCount;
        private readonly long? _fileSize;

        public ChunkedUploadProgress(string uploadId, long chunksCompleted, long bytesSaved, long retryCount, long? fileSize)
        {
            _uploadId = uploadId;
            _chunksCompleted = chunksCompleted;
            _bytesSaved = bytesSaved;
            _retryCount = retryCount;
            _fileSize = fileSize;
        }

        public string UploadId
        {
            get
            {
                return this._uploadId;
            }
        }

        public long ChunksCompleted
        {
            get
            {
                return this._chunksCompleted;
            }
        }

        public long BytesSaved
        {
            get
            {
                return this._bytesSaved;
            }
        }

        public long? FileSize
        {
            get
            {
                return this._fileSize;
            }
        }

        public long RetryCount
        {
            get
            {
                return this._retryCount;
            }
        }
    }
}
