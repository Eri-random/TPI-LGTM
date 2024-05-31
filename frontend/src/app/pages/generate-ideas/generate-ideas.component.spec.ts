import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GenerateIdeasComponent } from './generate-ideas.component';

describe('GenerarIdeasComponent', () => {
  let component: GenerateIdeasComponent;
  let fixture: ComponentFixture<GenerateIdeasComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [GenerateIdeasComponent]
    });
    fixture = TestBed.createComponent(GenerateIdeasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
