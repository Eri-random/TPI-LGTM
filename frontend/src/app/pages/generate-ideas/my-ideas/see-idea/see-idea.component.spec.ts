import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeeIdeaComponent } from './see-idea.component';

describe('VerIdeaComponent', () => {
  let component: SeeIdeaComponent;
  let fixture: ComponentFixture<SeeIdeaComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SeeIdeaComponent]
    });
    fixture = TestBed.createComponent(SeeIdeaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
