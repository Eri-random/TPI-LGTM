import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpinnerIdeaComponent } from './spinner-idea.component';

describe('SpinnerIdeaComponent', () => {
  let component: SpinnerIdeaComponent;
  let fixture: ComponentFixture<SpinnerIdeaComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SpinnerIdeaComponent]
    });
    fixture = TestBed.createComponent(SpinnerIdeaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
