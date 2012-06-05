namespace GameOfLife.GameLogic.Assets {
    /// <summary>
    /// An enum of responses that will occur when adding assets to the player object. 
    /// This gives an indication of whether or not an asset was successfully given to
    /// a player or not.
    /// </summary>
    public enum AssetResponse {
        AddedSuccessfully,
        HasNoPartner,
        HasPartnerAlready,
        CarFull,
        AlreadyOwnsHouse,
        CollectedAllPassportStamps,
        HouseAlreadyOwned,
        RemovedSuccessfully
    }
}
