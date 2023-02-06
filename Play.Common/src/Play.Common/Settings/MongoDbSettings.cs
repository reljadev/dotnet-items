namespace Play.Common.Settings {
    public class MongoDbSettings {
        public string Host { get; init; }
        public string DockerHost { get; init; }
        public int Port { get; init; }
        public string ConnectionString => EnvironmentDeterminer.IsRunningInContainer ? 
                                            $"mongodb://{DockerHost}:{Port}" :
                                            $"mongodb://{Host}:{Port}";
    }
}