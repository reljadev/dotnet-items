namespace Play.Common {
    public  static class EnvironmentDeterminer {
        public static bool IsRunningInContainer => 
                    bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var inDocker) && 
                    inDocker;
    }
}