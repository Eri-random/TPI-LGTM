import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VerIdeaComponent } from './ver-idea.component';

describe('VerIdeaComponent', () => {
  let component: VerIdeaComponent;
  let fixture: ComponentFixture<VerIdeaComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VerIdeaComponent]
    });
    fixture = TestBed.createComponent(VerIdeaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
