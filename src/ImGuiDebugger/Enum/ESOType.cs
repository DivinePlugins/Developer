namespace ImGuiDebugger.Enum;
internal enum ESOType
{
    // EconItem is an economy item.
    CSOEconItem = 1,
    // ItemRecipe is an item recipe.
    CSOItemRecipe = 5,
    // EconGameAccountClient is a economy game account client..
    CSOEconGameAccountClient = 7,
    // SelectedItemPreset is a selected item preset. # no longer exists in game files
    //CSOSelectedItemPreset = 35,
    // ItemPresetInstance is a instance of an item preset. # no longer exists in game files
    //CSOEconItemPresetInstance = 36,
    // DropRateBonus is an active drop rate bonus.
    CSOEconItemDropRateBonus = 38,
    // EventTicket is a ticket to an event.
    CSOEconItemEventTicket = 40,
    // ItemTournamentPassport is an item representing a tournament passport.
    CSOEconItemTournamentPassport = 42,
    // GameAccountClient is the DOTA game account for a client.
    CSODOTAGameAccountClient = 2002,
    // Party is a Dota 2 party.
    CSODOTAParty = 2003,
    // Lobby is a Dota 2 lobby.
    CSODOTALobby = 2004,
    // PartyInvite is an invite to a party.
    CSODOTAPartyInvite = 2006,
    // GameHeroFavorites are game hero favorites.
    CSODOTAGameHeroFavorites = 2007,
    // MapLocationState is the minimap location state.
    CSODOTAMapLocationState = 2008,
    // Tournament represents a tournament.
    CMsgDOTATournament = 2009,
    // PlayerChallenge represents a player challenge.
    CSODOTAPlayerChallenge = 2010,
    // LobbyInvite is an invitation to a lobby.
    CSODOTALobbyInvite = 2011,
    // GameAccountPlus.
    CSODOTAGameAccountPlus = 2012,
}
