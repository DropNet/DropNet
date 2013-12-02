using System;
using DropNet.Exceptions;
using DropNet.Models;

namespace DropNet.Helpers
{
    public class ChunkedUploadHelper
    {
        private const long DefaultMaxRetries = 100;
        private readonly DropNetClient _client;
        private readonly Func<long, byte[]> _chunkNeeded;
        private readonly string _path;
        private readonly Action<MetaData> _success;
        private readonly Action<DropboxException> _failure;
        private readonly Action<ChunkedUploadProgress> _progress;
        private readonly bool _overwrite;
        private readonly string _parentRevision;
        private readonly long? _fileSize;
        private readonly long? _maxRetries;
        private long _chunksCompleted;
        private long _chunksFailed;
        private ChunkedUpload _lastChunkUploaded;

        public ChunkedUploadHelper(DropNetClient client, Func<long, byte[]> chunkNeeded, string path, Action<MetaData> success, Action<DropboxException> failure, Action<ChunkedUploadProgress> progress, bool overwrite, string parentRevision, long? fileSize, long? maxRetries)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            if (chunkNeeded == null)
            {
                throw new ArgumentNullException("chunkNeeded");
            }

            if (success == null)
            {
                throw new ArgumentNullException("success");
            }

            if (failure == null)
            {
                throw new ArgumentNullException("failure");
            }

            _client = client;
            _chunkNeeded = chunkNeeded;
            _path = path;
            _success = success;
            _failure = failure;
            _progress = progress;
            _overwrite = overwrite;
            _parentRevision = parentRevision;
            _fileSize = fileSize;
            _maxRetries = maxRetries;
        }

        public void Start()
        {
            var firstChunk = _chunkNeeded.Invoke(0);
            var chunkLength = firstChunk.GetLength(0);
            if (chunkLength <= 0)
            {
                _failure.Invoke(new DropboxException("Aborting chunked upload because chunkNeeded function returned no data on first call."));
            }

            UpdateProgress(0, null);
            _client.StartChunkedUploadAsync(firstChunk, OnChunkSuccess, OnChunkedUploadFailure );
        }

        private void UpdateProgress(long offset, string uploadId)
        {
            if (_progress != null)
            {
                _progress.Invoke(new ChunkedUploadProgress(uploadId, _chunksCompleted, offset, _chunksFailed, _fileSize));
            }
        }

        private void OnChunkSuccess(ChunkedUpload chunkedUpload)
        {
            _chunksCompleted++;
            _lastChunkUploaded = chunkedUpload;
            UpdateProgress(chunkedUpload.Offset, chunkedUpload.UploadId);
            var offset = chunkedUpload.Offset;
            var nextChunk = _fileSize.GetValueOrDefault(long.MaxValue) > offset
                                ? _chunkNeeded.Invoke(offset)
                                : new byte[0];

            var chunkLength = nextChunk.GetLength(0);
            if (chunkLength > 0)
            {
                _client.AppendChunkedUploadAsync(chunkedUpload, nextChunk, OnChunkSuccess, OnChunkedUploadFailure);
            }
            else
            {
                _client.CommitChunkedUploadAsync(chunkedUpload, _path, _success, _failure, _overwrite, _parentRevision);
            }
        }

        private void OnChunkedUploadFailure(DropboxException dropboxException)
        {
            _chunksFailed++;
            if (_lastChunkUploaded != null && _chunksFailed <= _maxRetries.GetValueOrDefault(DefaultMaxRetries))
            {
                OnChunkSuccess(_lastChunkUploaded);
            }
            else
            {
                _failure.Invoke(dropboxException);
            }
        }
    }
}