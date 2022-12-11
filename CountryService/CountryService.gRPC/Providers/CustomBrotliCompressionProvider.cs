using System.IO.Compression;
using Grpc.Net.Compression;

namespace CountryService.gRPC.Providers;

public class CustomBrotliCompressionProvider : ICompressionProvider
{
    private readonly CompressionLevel? _compressionLevel;

    public string EncodingName => "br";

    public CustomBrotliCompressionProvider(CompressionLevel? compressionLevel)
    {
        _compressionLevel = compressionLevel;
    }

    public Stream CreateCompressionStream(Stream stream, CompressionLevel? compressionLevel)
    {
        var brotliStream = _compressionLevel.HasValue
            ? new BrotliStream(stream, compressionLevel ?? _compressionLevel.Value, true)
            : !_compressionLevel.HasValue && compressionLevel.HasValue
                ? new BrotliStream(stream, compressionLevel.Value, true)
                : new BrotliStream(stream, CompressionLevel.Fastest, true);

        return brotliStream;
    }

    public Stream CreateDecompressionStream(Stream stream)
    {
        return new BrotliStream(stream, CompressionMode.Decompress);
    }
}