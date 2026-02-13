import type { User } from "../types/User";

export interface UserSpotifyStats {
  user: User;
  totalTracks: number;
  totalArtists: number;
  totalAlbums: number;
}
