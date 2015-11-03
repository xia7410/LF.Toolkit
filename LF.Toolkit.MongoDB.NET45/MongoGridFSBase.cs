using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB
{
    public abstract class MongoGridFSBase : MongoStorageBase
    {
        public GridFSBucketOptions BucketOptions { get; private set; }

        public MongoGridFSBase(IMongoStorageConfig config, string databaseName, string bucketName, int chunkSizeBytes = 261120)
            : this(config, databaseName, bucketName, new GridFSBucketOptions
            {
                BucketName = bucketName,
                ChunkSizeBytes = chunkSizeBytes,
                WriteConcern = WriteConcern.WMajority,
                ReadPreference = ReadPreference.Secondary
            })
        {

        }

        public MongoGridFSBase(IMongoStorageConfig config, string databaseName, string bucketName, GridFSBucketOptions options)
            : base(config, databaseName, bucketName)
        {
            BucketOptions = options;
        }

        /// <summary>
        /// Uploads a file (or a new revision of a file) to GridFS.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<ObjectId> UploadFromBytesAsync(string filename, byte[] source, GridFSUploadOptions options = null)
        {
            var bucket = new GridFSBucket(base.GetDatabase(), BucketOptions);
            return bucket.UploadFromBytesAsync(filename, source, options);
        }

        /// <summary>
        /// Uploads a file (or a new revision of a file) to GridFS.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<ObjectId> UploadFromStreamAsync(string filename, Stream source, GridFSUploadOptions options = null)
        {
            var bucket = new GridFSBucket(base.GetDatabase(), BucketOptions);
            return bucket.UploadFromStreamAsync(filename, source, options);
        }

        /// <summary>
        /// Opens a Stream that can be used by the application to write data to a GridFS file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<GridFSUploadStream> OpenUploadStreamAsync(string filename, GridFSUploadOptions options = null)
        {
            var bucket = new GridFSBucket(base.GetDatabase(), BucketOptions);
            return bucket.OpenUploadStreamAsync(filename, options);
        }

        /// <summary>
        /// Downloads a file stored in GridFS and returns it as a byte array.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<byte[]> DownloadAsBytesAsync(ObjectId id, GridFSDownloadOptions options = null)
        {
            var bucket = new GridFSBucket(base.GetDatabase(), BucketOptions);
            return bucket.DownloadAsBytesAsync(id, options);
        }

        /// <summary>
        /// Downloads a file stored in GridFS and returns it as a byte array.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<byte[]> DownloadAsBytesByNameAsync(string filename, GridFSDownloadByNameOptions options = null)
        {
            var bucket = new GridFSBucket(base.GetDatabase(), BucketOptions);
            return bucket.DownloadAsBytesByNameAsync(filename, options);
        }

        /// <summary>
        /// Downloads a file stored in GridFS and writes the contents to a stream.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="destination"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task DownloadToStreamAsync(ObjectId id, Stream destination, GridFSDownloadOptions options = null)
        {
            var bucket = new GridFSBucket(base.GetDatabase(), BucketOptions);
            return bucket.DownloadToStreamAsync(id, destination, options);
        }

        /// <summary>
        /// Downloads a file stored in GridFS and writes the contents to a stream.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="destination"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task DownloadToStreamAsync(string filename, Stream destination, GridFSDownloadByNameOptions options = null)
        {
            var bucket = new GridFSBucket(base.GetDatabase(), BucketOptions);
            return bucket.DownloadToStreamByNameAsync(filename, destination, options);
        }

        /// <summary>
        /// Deletes a file from GridFS.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAsync(ObjectId id)
        {
            var bucket = new GridFSBucket(base.GetDatabase(), BucketOptions);
            return bucket.DeleteAsync(id);
        }

        /// <summary>
        /// Renames a GridFS file.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newFilename"></param>
        /// <returns></returns>
        public Task RenameAsync(ObjectId id, string newFilename)
        {
            var bucket = new GridFSBucket(base.GetDatabase(), BucketOptions);
            return bucket.RenameAsync(id, newFilename);
        }

        /// <summary>
        ///  Finds matching entries from the files collection.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<IAsyncCursor<GridFSFileInfo>> FindAsync(FilterDefinition<GridFSFileInfo> filter, GridFSFindOptions options = null)
        {
            var bucket = new GridFSBucket(base.GetDatabase(), BucketOptions);
            return bucket.FindAsync(filter, options);
        }
    }
}
