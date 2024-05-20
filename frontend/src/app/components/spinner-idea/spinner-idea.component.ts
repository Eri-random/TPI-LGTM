import { Component , OnInit } from '@angular/core';
import { SpinnerService } from 'src/app/services/spinner.service';

@Component({
  selector: 'app-spinner-idea',
  templateUrl: './spinner-idea.component.html',
  styleUrls: ['./spinner-idea.component.css']
})
export class SpinnerIdeaComponent implements OnInit{
  isLoadingIdea: boolean = false;

  constructor(private spinnerService: SpinnerService) {}

  ngOnInit(): void {
    this.spinnerService.loadingIdea$.subscribe((loading: boolean) => {
      this.isLoadingIdea = loading;
    });
  }
}
