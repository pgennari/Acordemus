import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DownloadRegiment } from "../../../featured/components/download-regiment/download-regiment";

@Component({
  selector: 'app-home',
  imports: [RouterLink, DownloadRegiment],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {

}
